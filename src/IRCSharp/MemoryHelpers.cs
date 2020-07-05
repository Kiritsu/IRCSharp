using System;

namespace IRCSharp
{
    public static class MemoryHelpers
    {
        public static ReadOnlyMemory<T> Concat<T>(ReadOnlyMemory<T> left, ReadOnlyMemory<T> right)
        {
            Memory<T> memory = new T[left.Length + right.Length];
            
            left.CopyTo(memory[..left.Length]);
            right.CopyTo(memory[left.Length..]);

            return memory;
        }
    }
}