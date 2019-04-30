using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IRCSharp.Entities.Models;

namespace IRCSharp.Entities
{
    public sealed class Channel
    {
        /// <summary>
        ///     Name of the channel.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        ///     Topic of the channel.
        /// </summary>
        public Topic Topic { get; internal set; }

        /// <summary>
        ///     Modes sets on the channel.
        /// </summary>
        public ReadOnlyCollection<char> Modes { get; }

        internal readonly List<char> _modes;

        /// <summary>
        ///     Password of the channel.
        /// </summary>
        public string Key { get; internal set; }

        /// <summary>
        ///     Limit of users in the channel.
        /// </summary>
        public int Limit { get; internal set; }

        /// <summary>
        ///     Users in the channel.
        /// </summary>
        public ReadOnlyCollection<ChannelUser> Users { get; }

        internal readonly List<ChannelUser> _users;

        /// <summary>
        ///     <see cref="DateTimeOffset"/> of the creation of the channel.
        /// </summary>
        public DateTimeOffset CreatedAt { get; internal set; }

        internal Channel()
        {
            Topic = new Topic();

            _modes = new List<char>();
            Modes = new ReadOnlyCollection<char>(_modes);

            _users = new List<ChannelUser>();
            Users = new ReadOnlyCollection<ChannelUser>(_users);
        }
    }
}
