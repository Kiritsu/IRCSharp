using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IRCSharp.Entities
{
    public class User
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

        internal readonly List<Channel> _channels;

        internal readonly IRCClient _client;

        internal User(IRCClient client)
        {
            _client = client;

            _channels = new List<Channel>();
            Channels = new ReadOnlyCollection<Channel>(_channels);
        }

        /// <summary>
        ///     Checks wether the given object is equal to the <see cref="User"/>.
        /// </summary>
        /// <param name="obj">Object to compare.</param>
        public override bool Equals(object obj)
        {
            if (obj is User user)
            {
                return user.Username == Username;
            }

            return false;
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
