using System.Collections.Generic;
using IRCSharp.Entities;

namespace IRCSharp.EventArgs
{
    public sealed class ChannelModesUpdatedEventArgs : EventArgs
    {
        /// <summary>
        ///     Channel that has their modes updated.
        /// </summary>
        public Channel Channel { get; internal set; }

        /// <summary>
        ///     Modes added.
        /// </summary>
        public IReadOnlyList<char> ModesAdded { get; internal set; }

        /// <summary>
        ///     Modes removed.
        /// </summary>
        public IReadOnlyList<char> ModesRemoved { get; internal set; }
        
        /// <summary>
        ///     Dictionary of mode by if it has been removed or not, and the extra args to it.
        /// </summary>
        public IReadOnlyDictionary<char, (char, string)> ModesArgs { get; internal set; }
    }
}
