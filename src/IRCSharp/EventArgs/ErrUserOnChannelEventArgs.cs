using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrUserOnChannel
    public sealed class ErrUserOnChannelEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string Channel { get; internal set; }

        internal ErrUserOnChannelEventArgs()
        {
             
        }
    }
}