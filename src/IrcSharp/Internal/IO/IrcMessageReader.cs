using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;

namespace IrcSharp.Internal.IO;

internal interface IIrcMessageReader
{
    IAsyncEnumerable<RawIrcMessage> ReadMessagesAsync(
        int maximumMessageSize = Consts.StandardMaximumMessageSize,
        CancellationToken cancellationToken = default);
}

internal sealed class IrcMessageReader(Stream stream) : IAsyncDisposable, IIrcMessageReader
{
    private readonly PipeReader _pipeReader = PipeReader.Create(stream);
    private int _readerActive;

    public async IAsyncEnumerable<RawIrcMessage> ReadMessagesAsync(
        int maximumMessageSize = Consts.StandardMaximumMessageSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _readerActive, 1, 0) != 0)
        {
            throw new InvalidOperationException("The reader is already active");
        }

        try
        {
            while (true)
            {
                var readResult = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                if (readResult.IsCanceled)
                {
                    throw new OperationCanceledException(cancellationToken);
                }
                
                var buffer = readResult.Buffer;

                if (buffer.Length > maximumMessageSize)
                {
                    var testReader = new SequenceReader<byte>(buffer);
                    if (!testReader.TryReadTo(out ReadOnlySequence<byte> _, Consts.Crlf))
                    {
                        throw new InvalidOperationException(
                            $"Received {buffer.Length} bytes without finding message delimiter (max: {maximumMessageSize})");
                    }
                }
                
                while (true)
                {
                    var sequenceReader = new SequenceReader<byte>(buffer);
                    if (!sequenceReader.TryReadTo(out ReadOnlySequence<byte> localLine, Consts.Crlf))
                    {
                        break;
                    }
                    
                    if (localLine.Length == 0)
                    {
                        // skip empty lines
                        buffer = buffer.Slice(sequenceReader.Position);
                        continue;
                    }

                    var message = IrcMessagePool.Rent(localLine);
                    buffer = buffer.Slice(sequenceReader.Position);
                
                    yield return message;
                }
            
                _pipeReader.AdvanceTo(buffer.Start, buffer.End);
                
                if (readResult.IsCompleted)
                {
                    break;
                }
            }
        }
        finally
        {
            Interlocked.Exchange(ref _readerActive, 0);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _pipeReader.CompleteAsync().ConfigureAwait(false);
    }
}