using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public class UserLeftEventArgs : EventArgs
    {
        /// <summary>
        ///     User that left the channel.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Channel on which the user left.
        /// </summary>
        public Channel Channel { get; internal set; }

        internal UserLeftEventArgs()
        {

        }
    }
}
