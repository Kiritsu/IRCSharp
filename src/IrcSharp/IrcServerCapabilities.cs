using System.Collections.ObjectModel;

namespace IrcSharp;

public class IrcServerCapabilities
{
    private readonly Dictionary<string, string> _capabilities = new();

    public IReadOnlyDictionary<string, string> Capabilities { get; init; }
    
    internal IrcServerCapabilities()
    {
        Capabilities = new ReadOnlyDictionary<string, string>(_capabilities);
    }

    public void Append(string capability)
    {
        
    }

    public void Append(IEnumerable<string> capabilities)
    {
        
    }
}