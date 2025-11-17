using System.Buffers;
using System.Text;

namespace IrcSharp.Internal;

public sealed class RawIrcMessage : IDisposable
{
    private byte[]? _buffer;
    private ushort _length;
    
    private (int start, int length)? _tagsRange;
    private (int start, int length)? _prefixRange;
    private (int start, int length)? _commandRange;
    private (int start, int length)? _paramsRange;
    private (int start, int length)? _trailingRange;
    
    // @tags :prefix COMMAND [params] :trailing
    // crlf is not included in the sequence
    internal void Initialize(ReadOnlySequence<byte> sequence)
    {
        _length = (ushort)sequence.Length;
        _buffer = ArrayPool<byte>.Shared.Rent(_length);
        sequence.CopyTo(_buffer);
    }
    
    internal void Reset()
    {
        if (_buffer != null)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
        }

        _tagsRange = null;
        _prefixRange = null;
        _commandRange = null;
        _paramsRange = null;
        _trailingRange = null;
    }

    public ReadOnlySpan<byte> AsSpan() => _buffer.AsSpan(0, _length);

    private bool TryGetTagsRange(out int startIndex, out int sequenceLength)
    {
        if (_tagsRange.HasValue)
        {
            (startIndex, sequenceLength) = _tagsRange.Value;
            return sequenceLength > 0;
        }
        
        startIndex = 0;
        sequenceLength = 0;
    
        var span = AsSpan();
        if (span.Length == 0)
        {
            return false;
        }
    
        if (span[0] != (byte)'@')
        {
            return false;
        }
    
        var nextSpace = span.IndexOf((byte)' ');
        sequenceLength = nextSpace == -1 
            ? span.Length 
            : nextSpace;
    
        // trim trailing spaces
        while (sequenceLength > 0 && span[sequenceLength - 1] == (byte)' ')
        {
            sequenceLength--;
        }

        return true;
    }
    
    public ReadOnlySpan<byte> GetTags()
    {
        return !TryGetTagsRange(out var startIndex, out var sequenceLength) 
            ? ReadOnlySpan<byte>.Empty 
            : AsSpan().Slice(startIndex, sequenceLength);
    }
    
    private bool TryGetPrefixRange(out int startIndex, out int sequenceLength)
    {
        if (_prefixRange.HasValue)
        {
            (startIndex, sequenceLength) = _prefixRange.Value;
            return sequenceLength > 0;
        }
        
        startIndex = 0;
        sequenceLength = 0;

        var originalSpan = AsSpan();
        if (TryGetTagsRange(out var tagsStartIndex, out var tagsSequenceLength))
        {
            startIndex = tagsStartIndex + tagsSequenceLength + 1;
        }
        
        // skip left spaces
        while (startIndex < originalSpan.Length && originalSpan[startIndex] == (byte)' ')
        {
            startIndex++;
        }
        
        if (startIndex >= originalSpan.Length)
        {
            return false;
        }
    
        // get the prefix in the next part of the sequence (until space is found)
        var span = originalSpan[startIndex..];
        if (span.Length == 0 || span[0] != (byte)':')
        {
            // no prefix present or malformed message
            return false;
        }
    
        var nextSpace = span.IndexOf((byte)' ');
        sequenceLength = nextSpace == -1 
            ? span.Length // message ends with prefix, no command and parameters.
            : nextSpace;
    
        // Trim trailing spaces
        while (sequenceLength > 0 && span[sequenceLength - 1] == (byte)' ')
        {
            sequenceLength--;
        }
    
        return true;
    }

    public ReadOnlySpan<byte> GetPrefix()
    {
        return !TryGetPrefixRange(out var startIndex, out var sequenceLength) 
            ? ReadOnlySpan<byte>.Empty 
            : AsSpan().Slice(startIndex, sequenceLength);
    }
    
    private bool TryGetCommandRange(out int startIndex, out int sequenceLength)
    {
        if (_commandRange.HasValue)
        {
            (startIndex, sequenceLength) = _commandRange.Value;
            return sequenceLength > 0;
        }
        
        startIndex = 0;
        sequenceLength = 0;

        if (TryGetPrefixRange(out var prefixStartIndex, out var prefixSequenceLength))
        {
            startIndex = prefixStartIndex + prefixSequenceLength + 1;
        }
        else
        {
            startIndex = prefixStartIndex; // prefix not found but still has to slice everything before the position of the command
        }
    
        // skip any spaces before command
        var originalSpan = AsSpan();
        while (startIndex < originalSpan.Length && originalSpan[startIndex] == (byte)' ')
        {
            startIndex++;
        }

        if (startIndex >= originalSpan.Length)
        {
            return false;
        }
    
        var span = originalSpan[startIndex..];
        if (span.Length == 0)
        {
            // no command present or malformed message
            return false;
        }
    
        // get the command in the next part of the sequence (until space is found)
        var nextSpace = span.IndexOf((byte)' ');
        sequenceLength = nextSpace == -1 
            ? span.Length // message ends with command without parameters. 
            : nextSpace;
    
        // trim trailing spaces
        while (sequenceLength > 0 && span[sequenceLength - 1] == (byte)' ')
        {
            sequenceLength--;
        }
    
        return true;
    }
    
    public ReadOnlySpan<byte> GetCommand()
    {
        return !TryGetCommandRange(out var startIndex, out var sequenceLength) 
            ? ReadOnlySpan<byte>.Empty 
            : AsSpan().Slice(startIndex, sequenceLength);
    }
    
    private bool TryGetParamsRange(out int startIndex, out int sequenceLength)
    {
        if (_paramsRange.HasValue)
        {
            (startIndex, sequenceLength) = _paramsRange.Value;
            return sequenceLength > 0;
        }
        
        startIndex = 0;
        sequenceLength = 0;
    
        if (TryGetCommandRange(out var commandStartIndex, out var commandSequenceLength))
        {
            startIndex = commandStartIndex + commandSequenceLength + 1;
        }
        else
        {
            startIndex = commandStartIndex; // command not found but still has to slice everything before the position of the params
        }
    
        // skip any spaces before params
        var originalSpan = AsSpan();
        while (startIndex < originalSpan.Length && originalSpan[startIndex] == (byte)' ')
        {
            startIndex++;
        }
    
        if (startIndex >= originalSpan.Length)
        {
            return false;
        }
    
        var span = originalSpan[startIndex..];
        if (span.Length == 0)
        {
            // no params present or malformed message
            return false;
        }
    
        // get the params in the next part of the sequence (until colon is found)
        var nextColon = span.IndexOf((byte)':');
        sequenceLength = nextColon == -1 
            ? span.Length // message ends with parameters and no trailing values. 
            : nextColon;
    
        // trim trailing spaces
        while (sequenceLength > 0 && span[sequenceLength - 1] == (byte)' ')
        {
            sequenceLength--;
        }
    
        return true;
    }
    
    public ReadOnlySpan<byte> GetParams()
    {
        return !TryGetParamsRange(out var startIndex, out var sequenceLength) 
            ? ReadOnlySpan<byte>.Empty 
            : AsSpan().Slice(startIndex, sequenceLength);
    }
    
    private bool TryGetTrailingRange(out int startIndex, out int sequenceLength)
    {
        if (_trailingRange.HasValue)
        {
            (startIndex, sequenceLength) = _trailingRange.Value;
            return sequenceLength > 0;
        }
        
        startIndex = 0;
        sequenceLength = 0;
    
        if (TryGetParamsRange(out var paramsStartIndex, out var paramsSequenceLength))
        {
            startIndex = paramsStartIndex + paramsSequenceLength;
        }
        else
        {
            startIndex = paramsStartIndex; // params not found but still has to slice everything before the position of the trailing values
        }
    
        var originalSpan = AsSpan();
    
        // skip any spaces before the trailing marker
        while (startIndex < originalSpan.Length && originalSpan[startIndex] == (byte)' ')
        {
            startIndex++;
        }
    
        if (startIndex >= originalSpan.Length)
        {
            return false;
        }
    
        var span = originalSpan[startIndex..];
    
        // check for trailing marker ':'
        if (span.Length == 0 || span[0] != (byte)':')
        {
            // no params present or malformed message
            return false;
        }

        sequenceLength = span.Length;
        return true;
    }

    public ReadOnlySpan<byte> GetTrailing()
    {
        return !TryGetTrailingRange(out var startIndex, out var sequenceLength) 
            ? ReadOnlySpan<byte>.Empty 
            : AsSpan().Slice(startIndex, sequenceLength);
    }

    public override string ToString()
    {
        return Encoding.UTF8.GetString(AsSpan());
    }
    
    public void Dispose()
    {
        IrcMessagePool.Return(this);
    }
}