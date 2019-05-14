using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace IRCSharp.Entities
{
    public class User : IEquatable<User>
    {
        /// <summary>
        ///     Identd of the <see cref="User"/>.
        /// </summary>
        public virtual string Identd { get; internal set; }

        /// <summary>
        ///     Username of the <see cref="User"/>.
        /// </summary>
        public virtual string Username { get; internal set; }

        /// <summary>
        ///     Host of the <see cref="User"/>.
        /// </summary>
        public virtual string Host { get; internal set; }

        /// <summary>
        ///     Full host of the <see cref="User"/>. Username!Identd@Host
        /// </summary>
        public string FullHost => $"{Username}!{Identd}@{Host}";

        /// <summary>
        ///     Realname of the <see cref="User"/>.
        /// </summary>
        public virtual string Realname { get; internal set; }

        /// <summary>
        ///     Idle time of the <see cref="User"/>.
        /// </summary>
        public virtual TimeSpan Idle { get; internal set; }

        /// <summary>
        ///     Signon time of the <see cref="User"/>.
        /// </summary>
        public virtual DateTimeOffset Signon { get; internal set; }

        /// <summary>
        ///     Server on which the <see cref="User"/> is connected to.
        /// </summary>
        public virtual string Server { get; internal set; }

        /// <summary>
        ///     Indicates if the <see cref="User"/> is an IRC Operator.
        /// </summary>
        public virtual bool IRCOperator { get; internal set; }

        /// <summary>
        ///     Host of the user.
        /// </summary>
        public virtual string ReverseIp { get; internal set; }

        /// <summary>
        ///     Ip of the user.
        /// </summary>
        public virtual string Ip { get; internal set; }

        /// <summary>
        ///     Channels of the user.
        /// </summary>
        public virtual ReadOnlyCollection<Channel> Channels { get; }

        internal readonly ConcurrentDictionary<string, List<string>> _channelMessages;

        internal readonly List<Channel> _channels;

        internal readonly IRCClient _client;

        internal User(IRCClient client)
        {
            _client = client;

            _channelMessages = new ConcurrentDictionary<string, List<string>>();

            _channels = new List<Channel>();
            Channels = new ReadOnlyCollection<Channel>(_channels);
        }

        /// <summary>
        ///     Gets the cached messages sent by the current <see cref="User"/> on the specified channel or dm.
        /// </summary>
        /// <param name="location">Target channel name or username.</param>
        /// <remarks>If you request messages on a channel, please prefix it with #</remarks>
        public ImmutableArray<string> GetMessages(string location)
        {
            if (_channelMessages.TryGetValue(location, out var list))
            {
                return list.ToImmutableArray();
            }

            return ImmutableArray<string>.Empty;
        }

        /// <summary>
        ///     Checks wether the given object is equal to the <see cref="User"/>.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        /// <summary>
        ///     Checks wether the given object is equal to the <see cref="User"/>.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        public bool Equals(User user)
        {
            if (user is null)
            {
                return false;
            }

            return user.Username == Username;
        }

        /// <summary>
        ///     Checks wether the two <see cref="User"/> are equal.
        /// </summary>
        /// <param name="u1">Left User</param>
        /// <param name="u1">Right User</param>
        public static bool operator ==(User u1, User u2)
        {
            return u1.Equals(u2);
        }

        /// <summary>
        ///     Checks wether the two <see cref="User"/> are not equal.
        /// </summary>
        /// <param name="u1">Left User</param>
        /// <param name="u1">Right User</param>
        public static bool operator !=(User u1, User u2)
        {
            return !(u1 == u2);
        }

        /// <summary>
        ///     Gets the HashCode of the <see cref="User"/>.
        /// </summary>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Identd);
            hash.Add(Username);
            hash.Add(Host);
            hash.Add(FullHost);
            hash.Add(Realname);
            hash.Add(Idle);
            hash.Add(Signon);
            hash.Add(Server);
            hash.Add(IRCOperator);
            hash.Add(ReverseIp);
            hash.Add(Ip);
            hash.Add(Channels);
            hash.Add(_channels);
            return hash.ToHashCode();
        }
    }
}
