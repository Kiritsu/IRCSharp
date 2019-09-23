using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class UserKickedEventArgs : EventArgs
    {
        /// <summary>
        ///     Channel that has their modes updated.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     <see cref="ChannelUser"/> that kicked the <see cref="Kicked"/>.
        /// </summary>
        public ChannelUser Kicker { get; internal set; }

        /// <summary>
        ///     <see cref="User"/> that has been kicked.
        /// </summary>
        public User Kicked { get; internal set; }

        /// <summary>
        ///     Reason of the kick.
        /// </summary>
        public string Reason { get; internal set; }

        internal UserKickedEventArgs()
        {

        }
    }
}
