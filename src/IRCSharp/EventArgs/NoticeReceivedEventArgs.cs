using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class NoticeReceivedEventArgs : EventArgs
    {
        /// <summary>
        ///     Sender of the message.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Message that has been sent.
        /// </summary>
        public string Message { get; internal set; }

        internal NoticeReceivedEventArgs()
        {

        }
    }
}
