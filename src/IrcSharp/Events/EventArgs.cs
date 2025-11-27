using IrcSharp.Attributes;

namespace IrcSharp.Events;

[IrcSharpEvent("Empty")]
[IrcSharpEvent("Generic", "IrcMessage:Origin")]
[IrcSharpEvent("UnknownMessage", "IrcMessage:Origin", "string:Message")]
[IrcSharpEvent("Ping", "IrcMessage:Origin", "string?:TrailingValue")]
[IrcSharpEvent("RplIsupport", "IrcMessage:Origin", "IReadOnlyDictionary<string, string?>:Capabilities")]
public abstract class EventArgs;