using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        ///     Channel on which the message has been sent. Null if the message was sent in private.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     Sender of the message. Can be a <see cref="ChannelUser"/> if <see cref="Channel"/> is not null.
        /// </summary>
        public User User { get; internal set; }

        /// <summary>
        ///     Message that has been sent.
        /// </summary>
        public string Message { get; internal set; }
    }
}
