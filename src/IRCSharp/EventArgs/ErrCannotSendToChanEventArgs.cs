using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrCannotSendToChan
    public sealed class ErrCannotSendToChanEventArgs : EventArgs 
    { 
        public string ChannelName { get; internal set; }

        internal ErrCannotSendToChanEventArgs()
        {
             
        }
    }
}