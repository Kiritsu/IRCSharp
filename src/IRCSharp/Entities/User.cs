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
        public string Identd { get; internal set; }

        /// <summary>
        ///     Username of the <see cref="User"/>.
        /// </summary>
        public string Username { get; internal set; }

        /// <summary>
        ///     Host of the <see cref="User"/>.
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        ///     Full host of the <see cref="User"/>. Username!Identd@Host
        /// </summary>
        public string FullHost => $"{Username}!{Identd}@{Host}";

        /// <summary>
        ///     Realname of the <see cref="User"/>.
        /// </summary>
        public string Realname { get; internal set; }

        /// <summary>
        ///     Idle time of the <see cref="User"/>.
        /// </summary>
        public TimeSpan Idle { get; internal set; }

        /// <summary>
        ///     Signon time of the <see cref="User"/>.
        /// </summary>
        public DateTimeOffset Signon { get; internal set; }

        /// <summary>
        ///     Server on which the <see cref="User"/> is connected to.
        /// </summary>
        public string Server { get; internal set; }

        /// <summary>
        ///     Indicates if the <see cref="User"/> is an IRC Operator.
        /// </summary>
        public bool IRCOperator { get; internal set; }

        /// <summary>
        ///     Host of the user.
        /// </summary>
        public string ReverseIp { get; internal set; }

        /// <summary>
        ///     Ip of the user.
        /// </summary>
        public string Ip { get; internal set; }

        /// <summary>
        ///     Channels of the user.
        /// </summary>
        public ReadOnlyCollection<Channel> Channels { get; }

        internal readonly List<Channel> _channels;

        internal User()
        {
            _channels = new List<Channel>();
            Channels = new ReadOnlyCollection<Channel>(_channels);
        }
    }
}
