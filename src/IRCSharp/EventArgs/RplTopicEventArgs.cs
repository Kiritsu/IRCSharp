using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTopic
    public sealed class RplTopicEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string Topic { get; internal set; }

        internal RplTopicEventArgs()
        {
             
        }
    }
}