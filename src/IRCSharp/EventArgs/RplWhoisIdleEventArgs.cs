using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoisIdle
    public sealed class RplWhoisIdleEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string Seconds { get; internal set; }

        internal RplWhoisIdleEventArgs()
        {
             
        }
    }
}