using System;

namespace IRCSharp.Entities.Models
{
    public sealed class ChannelBan
    {
        /// <summary>
        ///     Gets the host that is banned from the channel.
        /// </summary>
        public string BannedHost { get; internal set; }

        /// <summary>
        ///     Gets the username that banned that host.
        /// </summary>
        public string BannedBy { get; internal set; }

        /// <summary>
        ///     Gets the date and time this host was banned.
        /// </summary>
        public DateTimeOffset BannedOn { get; internal set; }
    }
}
