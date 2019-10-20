using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class UserLeftEventArgs : EventArgs
    {
        /// <summary>
        ///     User that left the channel.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Channel on which the user left.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     Reason why the user left the channel. Empty if not specified.
        /// </summary>
        public string Reason { get; internal set; }

        internal UserLeftEventArgs()
        {

        }
    }
}
