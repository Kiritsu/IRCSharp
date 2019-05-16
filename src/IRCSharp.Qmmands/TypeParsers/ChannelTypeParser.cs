using System;
using System.Threading.Tasks;
using IRCSharp.Entities;
using Qmmands;

namespace IRCSharp.Qmmands.TypeParsers
{
    public sealed class ChannelTypeParser : TypeParser<Channel>
    {
        public override ValueTask<TypeParserResult<Channel>> ParseAsync(Parameter parameter, string value, CommandContext ctx, IServiceProvider provider)
        {
            var context = ctx as IRCCommandContext;
            if (context is null)
            {
                throw new ArgumentException($"Unexpected behavior: the context is not a {typeof(IRCCommandContext).Name}", nameof(context));
            }

            if (!context.Client.CachedChannels.TryGetValue(value, out var channel))
            {
                if (!context.Client.CachedChannels.TryGetValue(value.Insert(0, "#"), out channel))
                {
                    return new TypeParserResult<Channel>($"A channel with name {value} was not found.");
                }
            }

            return new TypeParserResult<Channel>(channel);
        }
    }
}
