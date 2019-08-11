using System;
using System.Linq;
using System.Threading.Tasks;
using IRCSharp.Entities;
using Qmmands;

namespace IRCSharp.Qmmands.TypeParsers
{
    public sealed class UserTypeParser<T> : TypeParser<T> where T : User
    {
        public override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, CommandContext ctx, IServiceProvider provider)
        {
            var context = ctx as IRCCommandContext;
            if (context is null)
            {
                throw new ArgumentException($"Unexpected behavior: the context is not a {typeof(IRCCommandContext).Name}", nameof(context));
            }

            T user = null;
            if (typeof(T) == typeof(ChannelUser))
            {
                if (context.Channel is null)
                {
                    throw new InvalidOperationException("You can't lookup a ChannelUser if you're not in a channel context.");
                }

                user = context.Channel.Users.FirstOrDefault(x => x.Username == value) as T;
            }
            else if (context.Client.Users.TryGetValue(value, out var usr))
            {
                user = usr as T;
            }

            if (user is null)
            {
                return new TypeParserResult<T>($"A user with name {value} was not found.");
            }

            return new TypeParserResult<T>(user);
        }
    }
}
