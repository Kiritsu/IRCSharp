using System;

namespace IRCSharp
{
    public class IRCConfiguration
    {
        /// <summary>
        ///     Remote server hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        ///     Remote server port.
        /// </summary>
        public ushort Port { get; set; } = 6667;

        /// <summary>
        ///     Default nickname.
        /// </summary>
        public string Nickname { get; set; } = "user";
        
        /// <summary>
        ///     User's identifier. Defaults to 'user'.
        /// </summary>
        public string Identd { get; set; } = "user";

        /// <summary>
        ///     User's realname. Defaults to 'IRCSharp'
        /// </summary>
        public string Realname { get; set; } = "IRCSharp";

        /// <summary>
        ///     Remote server password. Null if no password is required.
        /// </summary>
        public string Password { get; set; } = null;

        /// <summary>
        ///     Whether the client must try to reconnect when connection is lost.
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        ///     Defines the time before without any communication / ping-pong to consider a timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
    }
}