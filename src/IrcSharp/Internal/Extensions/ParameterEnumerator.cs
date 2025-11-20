namespace IrcSharp.Internal.Extensions;

#pragma warning disable CA1815
public struct ParameterEnumerator
#pragma warning restore CA1815
{
    private readonly ReadOnlyMemory<byte> _params;
    private int _position;

    internal ParameterEnumerator(byte[] @params)
    {
        // forces copy of the span to ensure data integrity
        _params = @params.AsMemory();
        _position = 0;

        Current = default;
    }

    public ReadOnlyMemory<byte> Current { get; private set; }

    // todo: add tests
    public bool MoveNext()
    {
        // handles skipping leading spaces: "        param1  param2 param3   param4   "
        while (_position < _params.Length && _params.Span[_position] == (byte)' ')
        {
            _position++;
        }

        if (_position >= _params.Length)
        {
            Current = default;
            return false;
        }

        // finds next space that determines end of current parameter
        int start = _position;
        while (_position < _params.Length && _params.Span[_position] != (byte)' ')
        {
            _position++;
        }

        Current = _params.Slice(start, _position - start);
        return true;
    }
    
    public ParameterEnumerator GetEnumerator() => this;
}