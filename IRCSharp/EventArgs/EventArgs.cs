using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public class EventArgs
    {
        /// <summary>
        ///     Represents the <see cref="IRCClient"/> responsible of that <see cref="EventArgs"/>.
        /// </summary>
        public IRCClient Client { get; internal set; }

        /// <summary>
        ///     Represents the current <see cref="User"/>.
        /// </summary>
        public User CurrentUser { get; internal set; }

        internal EventArgs()
        {
        }
    }
}
