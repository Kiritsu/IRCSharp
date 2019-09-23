using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class NicknameChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     User that left the channel.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Old user's username.
        /// </summary>
        public string OldUsername { get; internal set; }

        /// <summary>
        ///     New user's username.
        /// </summary>
        public string NewUsername { get; internal set; }


    }
}
