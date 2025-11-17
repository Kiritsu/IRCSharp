using System.Text;

namespace IrcSharp.Internal;

internal readonly struct CommandKey : IEquatable<CommandKey>
{
    private readonly ulong _data1; // 8 bytes
    private readonly ulong _data2; // 8 bytes  
    private readonly byte _length;

    public CommandKey(ReadOnlySpan<byte> command)
    {
        // stores the command in two ulong values.
        _length = (byte)Math.Min(command.Length, 16);
        _data1 = 0;
        _data2 = 0;

        // pack first 8 bytes
        for (int i = 0; i < Math.Min(command.Length, 8); i++)
        {
            _data1 |= (ulong)command[i] << (i * 8);
        }

        // pack next 8 bytes
        for (int i = 8; i < command.Length && i < 16; i++)
        {
            _data2 |= (ulong)command[i - 8] << ((i - 8) * 8);
        }
    }

    public static CommandKey FromString(string command)
    {
        return new CommandKey(Encoding.UTF8.GetBytes(command));
    }

    public bool Equals(CommandKey other)
    {
        return _length == other._length &&
               _data1 == other._data1 &&
               _data2 == other._data2;
    }

    public override bool Equals(object? obj)
    {
        return obj is CommandKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_data1, _data2, _length);
    }

    public static bool operator ==(CommandKey left, CommandKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CommandKey left, CommandKey right)
    {
        return !(left == right);
    }
}