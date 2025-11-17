namespace IrcSharp;

internal static class Consts
{
    public const int StandardMaximumMessageSize = 512;
    public static readonly byte[] Crlf = "\r\n"u8.ToArray();

    public const string DefaultRealname = "ircsharp";
}