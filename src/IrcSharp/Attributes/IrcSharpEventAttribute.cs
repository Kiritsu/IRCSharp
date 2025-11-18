namespace IrcSharp.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class IrcSharpEventAttribute(string eventName, params string[] parameters) : Attribute
{
    public string EventName { get; } = eventName;

    public string[] Parameters { get; } = parameters;
}