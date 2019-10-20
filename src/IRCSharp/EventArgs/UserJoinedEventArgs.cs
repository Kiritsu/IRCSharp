using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class UserJoinedEventArgs : EventArgs
    {
        /// <summary>
        ///     User that joined the channel.
        /// </summary>
        public ChannelUser User { get; internal set; }

        /// <summary>
        ///     Channel on which the user joined.
        /// </summary>
        public Channel Channel { get; internal set; }

        internal UserJoinedEventArgs()
        {

        }
    }
}
