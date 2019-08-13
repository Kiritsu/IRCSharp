using System;
using System.Linq;
using IRCSharp.Entities;
using IRCSharp.EventArgs;
using Qmmands;

namespace IRCSharp.Qmmands
{
    public sealed class IRCCommandContext : CommandContext
    {
        /// <summary>
        ///     Gets the <see cref="IRCClient"/>.
        /// </summary>
        public IRCClient Client { get; internal set; }

        /// <summary>
        ///     Gets the <see cref="Channel"/> the action has been executed in. 
        ///     Null if the message is private or a notice.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     Gets the author of the message.
        /// </summary>
        public User Author { get; internal set; }

        /// <summary>
        ///    Gets the user in a channel context. Null if <see cref="Channel"/> is null.
        /// </summary>
        public ChannelUser User { get; internal set; }

        /// <summary>
        ///     Gets the current user.
        /// </summary>
        public User CurrentUser { get; internal set; }

        /// <summary>
        ///     Gets the content that has been sent.
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        ///     Gets the prefix of the command.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        ///     Gets the dependency collection.
        /// </summary>
        public IServiceProvider Services { get; internal set; }

        public IRCCommandContext(MessageReceivedEventArgs e)
        {
            Client = e.Client;
            Channel = e.Channel;
            Author = e.User;
            CurrentUser = e.CurrentUser;
            Message = e.Message;

            if (Channel != null)
            {
                User = Channel.Users.FirstOrDefault(x => x == Author);
            }
        }

        public IRCCommandContext(MessageReceivedEventArgs e, IServiceProvider services) : this(e)
        {
            Services = services;   
        }
    }
}
