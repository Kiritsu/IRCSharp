using IrcSharp.Attributes;

namespace IrcSharp.Events;

[IrcSharpEvent("Empty")]
[IrcSharpEvent("UnknownMessage", "string:Message")]
[IrcSharpEvent("Ping", "string?:TrailingValue")]
[IrcSharpEvent("RplWelcome", "string:Message")]
[IrcSharpEvent("RplIsupport", "IReadOnlyDictionary<string, string?>:Capabilities")]
public abstract class EventArgs;