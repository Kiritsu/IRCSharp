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
                return (Encoding.UTF8.GetString(@this.Span), null);
            }
            
            while (position < @this.Length && @this.Span[position] != (byte)'=')
            {
                position++;
            }

            if (position == @this.Length - 1)
            {
                // no equal sign found, value-less parameter
                return (Encoding.UTF8.GetString(@this.Span), null);
            }
            
            // todo: test this
            return (Encoding.UTF8.GetString(@this.Span[..position]), Encoding.UTF8.GetString(@this.Span[(position + 1)..]));
        }
    }
}