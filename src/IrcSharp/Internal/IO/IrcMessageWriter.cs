using System.Buffers;
using System.Text;

namespace IrcSharp.Internal.IO;

internal interface IIrcMessageWriter
{
    Task WriteAsync(
        ReadOnlyMemory<char> message, 
        int maximumMessageSize = Consts.StandardMaximumMessageSize, 
        CancellationToken cancellationToken = default);
}

internal sealed class IrcMessageWriter(Stream stream) : IDisposable, IIrcMessageWriter
{
    private readonly SemaphoreSlim _writeLock = new(1);
    
    public async Task WriteAsync(
        ReadOnlyMemory<char> message, 
        int maximumMessageSize = Consts.StandardMaximumMessageSize, 
        CancellationToken cancellationToken = default)
    {
        await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            int maxBytes = Encoding.UTF8.GetMaxByteCount(message.Length) + 2;
            if (maxBytes > maximumMessageSize)
            {
                throw new ArgumentException(
                    $"Message exceeds max length: {maxBytes} > {maximumMessageSize}");
            }
            
            byte[] buffer = ArrayPool<byte>.Shared.Rent(maxBytes);
            try
            {
                int bytesWritten = Encoding.UTF8.GetBytes(message.Span, buffer.AsSpan());
                
                buffer[bytesWritten] = Consts.Crlf[0];
                buffer[bytesWritten + 1] = Consts.Crlf[1];
                
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
        int maximumMessageSize = Consts.StandardMaximumMessageSize, 
        CancellationToken cancellationToken = default)
    {
        return WriteAsync(message.AsMemory(), maximumMessageSize, cancellationToken);
    }

    public void Dispose()
    {
        _writeLock.Dispose();
    }
}