namespace IrcSharp.Internal.Extensions;

#pragma warning disable CA1815
public struct SeparatedByEnumerator
#pragma warning restore CA1815
{
    private readonly char _separator;
    private readonly ReadOnlyMemory<byte> _params;
    private int _position;

    internal SeparatedByEnumerator(byte[] @params, char separator)
    {
        _separator = separator;
        _params = @params.AsMemory();
        _position = 0;

        Current = default;
    }

    public ReadOnlyMemory<byte> Current { get; private set; }

    // todo: add tests
    public bool MoveNext()
    {
        // handles skipping leading separators (exemple here is with spaces): "        param1  param2 param3   param4   "
        while (_position < _params.Length && _params.Span[_position] == (byte)_separator)
        {
            _position++;
        }

        if (_position >= _params.Length)
        {
            Current = default;
            return false;
        }

        // finds next separator position that determines end of current parameter
        int start = _position;
        while (_position < _params.Length && _params.Span[_position] != (byte)' ')
        {
            _position++;
        }

        Current = _params.Slice(start, _position - start);
        return true;
    }
    
    public SeparatedByEnumerator GetEnumerator() => this;
}