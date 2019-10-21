namespace IRCSharp.Entities
{
    public class ServerInfo
    {
        /// <summary>
        ///     Server name or host on which you are connected to.
        /// </summary>
        public string Host { get; internal set; }
        
        /// <summary>
        ///     Version of the remote server.
        /// </summary>
        public string Version { get; internal set; }
        
        /// <summary>
        ///     Creation date of the remote server.
        /// </summary>
        public string CreationDate { get; internal set; }
        
        /// <summary>
        ///     Supported user modes of the remote server.
        /// </summary>
        public string UserModes { get; internal set; }
        
        /// <summary>
        ///     Supported channel modes of the remote server.
        /// </summary>
        public string ChannelModes { get; internal set; }
        
        /// <summary>
        ///     Supported modes of the remote server. However, this doesn't follow RFC2812
        ///     So we don't know what these mods are for. It could be modes that require parameter(s)
        ///     Such as +k (KEY), +l (LIMIT), +b (BAN). 
        /// </summary>
        public string UnsupportedModes { get; internal set; }
    }
}