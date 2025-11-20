using IrcSharp.Attributes;

namespace IrcSharp.Events;

[IrcSharpEvent("Empty")]
[IrcSharpEvent("UnknownMessage", "string:Message")]
[IrcSharpEvent("Ping", "string?:TrailingValue")]
[IrcSharpEvent("ServerCapability", "IReadOnlyList<string>:Capabilities")]
public abstract class EventArgs;