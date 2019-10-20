using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNoSuchChannel
    public sealed class ErrNoSuchChannelEventArgs : EventArgs 
    { 
        public string ChannelName { get; internal set; }

        internal ErrNoSuchChannelEventArgs()
        {
             
        }
    }
}