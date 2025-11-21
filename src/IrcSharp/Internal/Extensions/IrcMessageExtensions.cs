using System.Collections.Immutable;
using System.Text;

namespace IrcSharp.Internal.Extensions;

internal static class IrcMessageExtensions
{
    extension(RawIrcMessage @this)
    {
        public bool IsPing()
        {
            return @this.GetCommand().SequenceEqual("PING"u8);
        }

        public string GetPingToken()
        {
            // Handles PING :trailing & PING params
            var trailing = @this.GetTrailing();
            if (trailing.Length > 0)
            {
                return Encoding.UTF8.GetString(trailing[1..]);
            }
        
            var parameters = @this.GetParams();
            if (parameters.Length > 0)
            {
                return Encoding.UTF8.GetString(parameters);
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
                    builder.Add(Encoding.UTF8.GetString(tag.Span));
                }

                tags = builder.MoveToImmutable();
            }

            var prefixRaw = @this.GetPrefix();
            var prefix = prefixRaw.IsEmpty ? null : Encoding.UTF8.GetString(prefixRaw);
            
            var command = Encoding.UTF8.GetString(@this.GetCommand());
            
            var parametersRaw = @this.GetParams();
            var parameters = ImmutableArray<string>.Empty;
            if (!parameters.IsEmpty)
            {
                var builder = ImmutableArray.CreateBuilder<string>(parametersRaw.Count((byte)' ') + 1);
                var enumerator = new SeparatedByEnumerator(parametersRaw.ToArray(), ' ');
                foreach (var tag in enumerator)
                {
                    builder.Add(Encoding.UTF8.GetString(tag.Span));
                }

                parameters = builder.MoveToImmutable();
            }
            
            var trailingRaw = @this.GetTrailing();
            var trailing = trailingRaw.IsEmpty ? null : Encoding.UTF8.GetString(trailingRaw);

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