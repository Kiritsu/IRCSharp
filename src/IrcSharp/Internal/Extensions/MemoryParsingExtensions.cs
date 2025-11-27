using System.Text;

namespace IrcSharp.Internal.Extensions;

public static class MemoryParsingExtensions
{
    extension(ReadOnlyMemory<byte> @this)
    {
        public (string param1, string? param2) ParseParameter()
        {
            var position = 0;
            if (@this.Span[0] == '-')
            {
                return (@this.Span[1..].AsUtf8String(), null);
            }
            
            while (position < @this.Length && @this.Span[position] != (byte)'=')
            {
                position++;
            }

            if (position == @this.Length)
            {
                // no equal sign found, value-less parameter
                return (@this.Span.AsUtf8String(), null);
            }
            
            // todo: test this
            return (@this.Span[..position].AsUtf8String(), @this.Span[(position + 1)..].AsUtf8String());
        }
        
        public string AsUtf8String() 
            => Encoding.UTF8.GetString(@this.Span);
    }

    extension(ReadOnlySpan<byte> @this)
    {
        public string AsUtf8String() 
            => Encoding.UTF8.GetString(@this);
    }
}