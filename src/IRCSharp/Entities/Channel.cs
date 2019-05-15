using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using IRCSharp.Entities.Models;

namespace IRCSharp.Entities
{
    public sealed class Channel : IEquatable<Channel>
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
        
        /// <summary>
        ///     Messages cache on this <see cref="Channel"/>.
        /// </summary>
        public ReadOnlyCollection<string> Messages { get; }
        
        internal readonly List<string> _messages;
        
        internal readonly IRCClient _client;

        internal Channel(IRCClient client)
        {
            _client = client;

            Topic = new Topic();

            _modes = new List<char>();
            Modes = new ReadOnlyCollection<char>(_modes);

            _users = new List<ChannelUser>();
            Users = new ReadOnlyCollection<ChannelUser>(_users);
            
            _messages = new List<string>();
            Messages = new ReadOnlyCollection<string>(_messages);
        }

        /// <summary>
        ///     Checks wether the given object is equal to the <see cref="User"/>.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as Channel);
        }

        /// <summary>
        ///     Checks wether the given object is equal to the <see cref="User"/>.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        public bool Equals(Channel channel)
        {
            if (channel is null)
            {
                return false;
            }

            return channel.Name == Name;
        }

        /// <summary>
        ///     Checks wether the two <see cref="User"/> are equal.
        /// </summary>
        /// <param name="u1">Left User</param>
        /// <param name="u1">Right User</param>
        public static bool operator ==(Channel c1, Channel c2)
        {
            return c1.Equals(c2);
        }

        /// <summary>
        ///     Checks wether the two <see cref="User"/> are not equal.
        /// </summary>
        /// <param name="u1">Left User</param>
        /// <param name="u1">Right User</param>
        public static bool operator !=(Channel c1, Channel c2)
        {
            return !(c1 == c2);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Name);
            hash.Add(Topic);
            hash.Add(Modes);
            hash.Add(_modes);
            hash.Add(Key);
            hash.Add(Limit);
            hash.Add(Users);
            hash.Add(_users);
            hash.Add(CreatedAt);
            hash.Add(_client);
            return hash.ToHashCode();
        }
    }
}
