using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        ///     Channel on which the message has been sent.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     Sender of the message.
        /// </summary>
        public ChannelUser User { get; internal set; }

        /// <summary>
        ///     Message that has been sent.
        /// </summary>
        public string Message { get; internal set; }
    }
}
