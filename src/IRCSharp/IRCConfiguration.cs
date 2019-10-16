using System;

namespace IRCSharp
{
    public sealed class IRCConfiguration
    {
        /// <summary>
        ///     Remote server hostname.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        ///     Remote server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///     Default username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     User identifier. Default to 'user'.
        /// </summary>
        public string Identd { get; set; } = "user";

        /// <summary>
        ///     User realname. Default to 'IRCSharp'
        /// </summary>
        public string RealName { get; set; } = "IRCSharp";

        /// <summary>
        ///     Remote server password. Empty if no password is required.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Indicates wether <see cref="IRCClient"/> must throw on timeout. 
        ///     When set to false, it will try to reconnect.
        /// </summary>
        public bool ThrowsOnTimeout { get; set; }

        /// <summary>
        ///     Defines the amount of time to wait before considering the
        ///     connection with the remote server has been lost.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        ///     Host sent by the server.
        /// </summary>
        internal string Host { get; set; }

        /// <summary>
        ///     Chan modes that have parameters.
        /// </summary>
        internal char[] ComplexChanModes { get; set; }

        /// <summary>
        ///     Creates a <see cref="IRCConfiguration"/>
        /// </summary>
        public IRCConfiguration()
        {

        }

        /// <summary>
        ///     Creates a copy of the specified <see cref="IRCConfiguration"/>.
        /// </summary>
        /// <param name="copy">Instance of <see cref="IRCConfiguration"/> to copy.</param>
        public IRCConfiguration(IRCConfiguration copy)
        {
            Hostname = copy.Hostname;
            Port = copy.Port;
            Username = copy.Username;
            Identd = copy.Identd;
            RealName = copy.RealName;
            Password = copy.Password;
            ThrowsOnTimeout = copy.ThrowsOnTimeout;
        }
    }
}