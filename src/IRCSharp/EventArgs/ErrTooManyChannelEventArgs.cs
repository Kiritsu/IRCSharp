using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrTooManyChannel
    public sealed class ErrTooManyChannelEventArgs : EventArgs 
    { 
        public string ChannelName { get; internal set; }

        internal ErrTooManyChannelEventArgs()
        {
             
        }
    }
}