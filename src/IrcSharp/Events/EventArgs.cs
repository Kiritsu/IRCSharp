using IrcSharp.Attributes;

namespace IrcSharp.Events;

[IrcSharpEvent("Empty")]
[IrcSharpEvent("UnknownMessage", "string:Message")]
[IrcSharpEvent("Ping", "string?:TrailingValue")]
public abstract class EventArgs;