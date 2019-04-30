using IRCSharp.Entities.Enums;

namespace IRCSharp.Entities
{
    public sealed class ChannelUser : User
    {
        /// <summary>
        ///     Privileges the user has on this channel.
        /// </summary>
        public ChannelPrivilege Privileges { get; internal set; }

        internal ChannelUser(User baseUser)
        {
            Identd = baseUser.Identd;
            Username = baseUser.Identd;
            Host = baseUser.Host;
            Realname = baseUser.Realname;
            Idle = baseUser.Idle;
            Signon = baseUser.Signon;
            Server = baseUser.Server;
            IRCOperator = baseUser.IRCOperator;
            ReverseIp = baseUser.ReverseIp;
            Ip = baseUser.Ip;
            _channels.AddRange(baseUser.Channels);
        }
    }
}
