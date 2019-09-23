using System;

namespace IRCSharp.Entities.Models
{
    public sealed class Topic
    {
        /// <summary>
        ///     Content of the topic.
        /// </summary>
        public string Content { get; internal set; }

        /// <summary>
        ///     Author of the topic.
        /// </summary>
        public string Author { get; internal set; }

        /// <summary>
        ///     <see cref="DateTimeOffset"/> when the topic has been set.
        /// </summary>
        public DateTimeOffset SetAt { get; internal set; }

        internal Topic()
        {

        }
    }
}
