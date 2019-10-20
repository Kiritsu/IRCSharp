using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrChannelIsFull
    public sealed class ErrChannelIsFullEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }

        internal ErrChannelIsFullEventArgs()
        {
             
        }
    }
}