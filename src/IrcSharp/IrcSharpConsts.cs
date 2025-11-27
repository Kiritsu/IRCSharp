namespace IrcSharp;

public static class IrcSharpConsts
{
    public const int StandardMaximumSendMessageSize = 512;
    public const int StandardMaximumReceiveMessageSize = 512;

    /// <summary>
    /// The maximum message size that can be sent to the server. This includes client-side tags (4092 bytes) and the message itself (512 bytes) including CRLF.
    /// </summary>
    public const int ExtendedMaximumSendMessageSize = 4092 + StandardMaximumSendMessageSize;
    
    /// <summary>
    /// The maximum message size that can be received from the server. This includes server-side tags (4092 bytes), client-side tags (4092 bytes) and the message itself (512 bytes) including CRLF.
    /// </summary>
    public const int ExtendedMaximumReceiveMessageSize = 4092 + 4092 + StandardMaximumReceiveMessageSize;
    
    public static readonly byte[] Crlf = "\r\n"u8.ToArray();

    public const string DefaultRealname = "ircsharp";
}