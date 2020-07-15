using System;

namespace IRCSharp
{
    internal class PacketHelpers
    {
        public static readonly Memory<byte> NicknamePacketHeader = new[] {(byte) 'N', (byte) 'I', (byte) 'C', (byte) 'K', (byte) ' '};
        public static readonly Memory<byte> PassPacketHeader = new[] {(byte) 'P', (byte) 'A', (byte) 'S', (byte) 'S', (byte) ' '};
        public static readonly Memory<byte> LineEnding = new[] {(byte) '\n'};
        public static readonly Memory<byte> ByteZero = new[] {(byte) '\0'};
    }
}