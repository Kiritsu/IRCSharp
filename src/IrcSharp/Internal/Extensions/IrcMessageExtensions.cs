using System.Text;

namespace IrcSharp.Internal.Extensions;

internal static class IrcMessageExtensions
{
    extension(RawIrcMessage @this)
    {
        public bool IsPing()
        {
            return @this.GetCommand().SequenceEqual("PING"u8);
        }

        public string GetPingToken()
        {
            // Handles PING :trailing & PING params
            var trailing = @this.GetTrailing();
            if (trailing.Length > 0)
            {
                return Encoding.UTF8.GetString(trailing[1..]);
            }
        
            var parameters = @this.GetParams();
            if (parameters.Length > 0)
            {
                return Encoding.UTF8.GetString(parameters);
            }
        
            return string.Empty;
        }

        public ParameterEnumerator EnumerateParameters()
        {
            return new ParameterEnumerator(@this.GetParams().ToArray());
        }
    }
}