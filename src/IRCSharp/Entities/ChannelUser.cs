using System;
using System.Collections.ObjectModel;
using IRCSharp.Entities.Enums;

namespace IRCSharp.Entities
{
    public sealed class ChannelUser : User
    {
        /// <summary>
        ///     Privileges the user has on this channel.
        /// </summary>
        public ChannelPrivilege Privileges { get; internal set; }

        /// <summary>
        ///     Identd of the <see cref="User"/>.
        /// </summary>
        public override string Identd => _baseUser.Identd;

        /// <summary>
        ///     Username of the <see cref="User"/>.
        /// </summary>
        public override string Username => _baseUser.Username;

        /// <summary>
        ///     Host of the <see cref="User"/>.
        /// </summary>
        public override string Host => _baseUser.Host;

        /// <summary>
        ///     Realname of the <see cref="User"/>.
        /// </summary>
        public override string Realname => _baseUser.Realname;

        /// <summary>
        ///     Idle time of the <see cref="User"/>.
        /// </summary>
        public override TimeSpan Idle => _baseUser.Idle;

        /// <summary>
        ///     Signon time of the <see cref="User"/>.
        /// </summary>
        public override DateTimeOffset Signon => _baseUser.Signon;

        /// <summary>
        ///     Server on which the <see cref="User"/> is connected to.
        /// </summary>
        public override string Server => _baseUser.Server;

        /// <summary>
        ///     Indicates if the <see cref="User"/> is an IRC Operator.
        /// </summary>
        public override bool IRCOperator => _baseUser.IRCOperator;

        /// <summary>
        ///     Host of the user.
        /// </summary>
        public override string ReverseIp => _baseUser.ReverseIp;

        /// <summary>
        ///     Ip of the user.
        /// </summary>
        public override string Ip => _baseUser.Ip;

        /// <summary>
        ///     Whether the user is on away mode.
        /// </summary>
        public override bool IsAway => _baseUser.IsAway;

        /// <summary>
        ///     Away message of the user.
        /// </summary>
        public override string Away => _baseUser.Away;

        /// <summary>
        ///     Channels of the user.
        /// </summary>
        public override ReadOnlyCollection<Channel> Channels => _baseUser.Channels;

        internal readonly User _baseUser;
        internal readonly Channel _channel;

        internal ChannelUser(IRCClient client, User baseUser, Channel channel) : base(client) 
        {
            _baseUser = baseUser;
            _channel = channel;

            Privileges = ChannelPrivilege.Normal;
        }

        /// <summary>
        ///     Kicks the user from the channel.
        /// </summary>
        /// <param name="reason">Reason of the kick.</param>
        public void Kick(string reason = null)
        {
            if (reason is null)
            {
                _client.Send($"KICK {_channel.Name} {Username}");
            }
            else
            {
                _client.Send($"KICK {_channel.Name} {Username} :{reason}");
            }
        }
    }
}
