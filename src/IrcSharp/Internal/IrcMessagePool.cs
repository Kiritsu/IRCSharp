using System.Buffers;
using Microsoft.Extensions.ObjectPool;

namespace IrcSharp.Internal;

internal static class IrcMessagePool
{
    private static readonly ObjectPool<RawIrcMessage> Pool = ObjectPool.Create<RawIrcMessage>();

    public static RawIrcMessage Rent(ReadOnlySequence<byte> sequence)
    {
        var message = Pool.Get();
        message.Initialize(sequence);
        return message;
    }

    public static void Return(RawIrcMessage message)
    {
        message.Reset();
        Pool.Return(message);
    }
}