using System;
using System.Reflection;
using IRCSharp.Entities;
using IRCSharp.EventArgs;
using IRCSharp.Qmmands.TypeParsers;
using Qmmands;

namespace IRCSharp.Qmmands
{
    public static class IRCClientExtensions
    {
        public static ICommandService AddCommandService(this IRCClient client, string prefix, CommandServiceConfiguration config = default, IServiceProvider services = default)
        {
            var commands = new CommandService(config ?? new CommandServiceConfiguration());

            commands.AddTypeParser(new ChannelTypeParser());
            commands.AddTypeParser(new UserTypeParser<ChannelUser>());
            commands.AddTypeParser(new UserTypeParser<User>());

            commands.AddModules(Assembly.GetCallingAssembly());

            client.MessageReceived += Client_MessageReceived;

            return commands;

            void Client_MessageReceived(MessageReceivedEventArgs e)
            {
                var context = new IRCCommandContext(e, services);

                if (!CommandUtilities.HasPrefix(e.Message, prefix, out var output))
                {
                    return;
                }

                context.Prefix = prefix;

                commands.ExecuteAsync(output, context).GetAwaiter().GetResult();
            }
        }
    }
}
