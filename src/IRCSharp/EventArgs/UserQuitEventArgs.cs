using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public class UserQuitEventArgs : EventArgs
    {
        /// <summary>
        ///     User that left the channel.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Reason why the user quit IRC. Empty if not specified.
        /// </summary>
        public string Reason { get; internal set; }

        internal UserQuitEventArgs()
        {

        }
    }
}
