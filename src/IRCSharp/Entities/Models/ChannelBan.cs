using System;

namespace IRCSharp.Entities.Models
{
    public sealed class ChannelBan
    {
        /// <summary>
        ///     Gets the host that is banned from the channel.
        /// </summary>
        public string Host { get; internal set; }

        /// <summary>
        ///     Gets the username that banned that host.
        /// </summary>
        public string IssuedBy { get; internal set; }

        /// <summary>
        ///     Gets the date and time this host was banned.
        /// </summary>
        public DateTimeOffset IssuedOn { get; internal set; }

        internal ChannelBan()
        {

        }
    }
}
