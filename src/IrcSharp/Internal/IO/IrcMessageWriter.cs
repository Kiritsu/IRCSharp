using System.Buffers;
using System.Text;

namespace IrcSharp.Internal.IO;

internal interface IIrcMessageWriter
{
    Task WriteAsync(
        ReadOnlyMemory<char> message, 
        int maximumMessageSize = IrcSharpConsts.StandardMaximumSendMessageSize, 
        CancellationToken cancellationToken = default);
}

internal sealed class IrcMessageWriter(Stream stream) : IDisposable, IIrcMessageWriter
{
    private readonly SemaphoreSlim _writeLock = new(1);
    
    public async Task WriteAsync(
        ReadOnlyMemory<char> message, 
        int maximumMessageSize = IrcSharpConsts.StandardMaximumSendMessageSize, 
        CancellationToken cancellationToken = default)
    {
        await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            int actualBytes = Encoding.UTF8.GetByteCount(message.Span);
            if (actualBytes + 2 > maximumMessageSize)
            {
                throw new ArgumentException(
                    $"Message exceeds max length: {actualBytes + 2} > {maximumMessageSize}");
            }

            byte[] buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetMaxByteCount(message.Length) + 2);
            try
            {
                int bytesWritten = Encoding.UTF8.GetBytes(message.Span, buffer.AsSpan());
                
                buffer[bytesWritten] = IrcSharpConsts.Crlf[0];
                buffer[bytesWritten + 1] = IrcSharpConsts.Crlf[1];
                
                await stream.WriteAsync(buffer.AsMemory(0, bytesWritten + 2), cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer, clearArray: false);
            }
        }
        finally
        {
            _writeLock.Release();
        }
    }
    
    public Task WriteAsync(
        string message, 
        int maximumMessageSize = IrcSharpConsts.StandardMaximumSendMessageSize, 
        CancellationToken cancellationToken = default)
    {
        return WriteAsync(message.AsMemory(), maximumMessageSize, cancellationToken);
    }

    public void Dispose()
    {
        _writeLock.Dispose();
    }
}