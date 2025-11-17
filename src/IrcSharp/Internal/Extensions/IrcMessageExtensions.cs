using System.Text;

namespace IrcSharp.Internal.Extensions;

internal static class IrcMessageExtensions
{
    public static bool IsPing(this RawIrcMessage @this)
    {
        return @this.GetCommand().SequenceEqual("PING"u8);
    }
    
    public static string GetPingToken(this RawIrcMessage message)
    {
        // Handles PING :trailing & PING params
        var trailing = message.GetTrailing();
        if (trailing.Length > 0)
        {
            return Encoding.UTF8.GetString(trailing[1..]);
        }
        
        var parameters = message.GetParams();
        if (parameters.Length > 0)
        {
            return Encoding.UTF8.GetString(parameters);
        }
        
        return string.Empty;
    }
}