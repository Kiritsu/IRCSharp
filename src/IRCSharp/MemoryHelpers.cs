using System;

namespace IRCSharp
{
    public static class MemoryHelpers
    {
        /// <summary>
        ///     Concatenate two <see cref="ReadOnlyMemory{T}"/> together.
        /// </summary>
        /// <param name="left">Left part of the new <see cref="ReadOnlyMemory{T}"/></param>
        /// <param name="right">Right part of the new <see cref="ReadOnlyMemory{T}"/></param>
        /// <typeparam name="T">Elements in the memory range.</typeparam>
        /// <returns>A new range of memory containing the two given ranges of memory.</returns>
        public static ReadOnlyMemory<T> Concat<T>(ReadOnlyMemory<T> left, ReadOnlyMemory<T> right)
        {
            if (left.Length == 0 && right.Length == 0)
            {
                return Memory<T>.Empty;
            }
            
            if (left.Length == 0)
            {
                Memory<T> rightCopy = new T[right.Length];
                right.CopyTo(rightCopy);
                return rightCopy;
            }
            
            if (right.Length == 0)
            {
                Memory<T> leftCopy = new T[left.Length];
                left.CopyTo(leftCopy);
                return leftCopy;
            }
            
            Memory<T> memory = new T[left.Length + right.Length];
            
            left.CopyTo(memory[..left.Length]);
            right.CopyTo(memory[left.Length..]);

            return memory;
        }
    }
}