using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplAway
    public sealed class RplAwayEventArgs : EventArgs 
    { 
        public string Nick { get; internal set; }
        public string Message { get; internal set; }

        internal RplAwayEventArgs()
        {
             
        }
    }
}