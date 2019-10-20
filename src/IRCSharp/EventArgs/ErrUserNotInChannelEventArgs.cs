using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrUserNotInChannel
    public sealed class ErrUserNotInChannelEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string Channel { get; internal set; }

        internal ErrUserNotInChannelEventArgs()
        {
             
        }
    }
}