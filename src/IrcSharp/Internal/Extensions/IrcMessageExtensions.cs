using System.Collections.Immutable;
using System.Text;

namespace IrcSharp.Internal.Extensions;

internal static class IrcMessageExtensions
{
    extension(RawIrcMessage @this)
    {
        public string GetPingToken()
        {
            // Handles PING :trailing & PING params
            var trailing = @this.GetTrailing();
            if (trailing.Length > 0)
            {
                return trailing.AsUtf8String();
            }
        
            var parameters = @this.GetParams();
            if (parameters.Length > 0)
            {
                return parameters.AsUtf8String();
            }
        
            return string.Empty;
        }

        public SeparatedByEnumerator EnumerateParameters()
        {
            return new SeparatedByEnumerator(@this.GetParams().ToArray(), ' ');
        }

        public IrcMessage ToIrcMessage()
        {
            var tagsRaw = @this.GetTags();
            var tags = ImmutableArray<string>.Empty;
            if (!tagsRaw.IsEmpty)
            {
                var builder = ImmutableArray.CreateBuilder<string>(tagsRaw.Count((byte)';') + 1);
                var enumerator = new SeparatedByEnumerator(tagsRaw.ToArray(), ';');
                foreach (var tag in enumerator)
                {
                    builder.Add(tag.Span.AsUtf8String());
                }

                tags = builder.MoveToImmutable();
            }

            var prefixRaw = @this.GetPrefix();
            var prefix = prefixRaw.IsEmpty ? null : prefixRaw.AsUtf8String();
            
            var command = @this.GetCommand().AsUtf8String();
            
            var parametersRaw = @this.GetParams();
            var parameters = ImmutableArray<string>.Empty;
            if (!parametersRaw.IsEmpty)
            {
                var builder = ImmutableArray.CreateBuilder<string>(parametersRaw.Count((byte)' ') + 1);
                var enumerator = new SeparatedByEnumerator(parametersRaw.ToArray(), ' ');
                foreach (var tag in enumerator)
                {
                    builder.Add(tag.Span.AsUtf8String());
                }

                parameters = builder.MoveToImmutable();
            }
            
            var trailingRaw = @this.GetTrailing();
            var trailing = trailingRaw.IsEmpty ? null : trailingRaw.AsUtf8String();

            return new IrcMessage
            {
                Tags = tags,
                Prefix = prefix ?? string.Empty,
                Command = command,
                Parameters = parameters,
                Trailing = trailing ?? string.Empty
            };
        }
    }
}