using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplBounce
    public sealed class RplBounceEventArgs : EventArgs 
    { 
        public string ServerName { get; internal set; }
        public string Port { get; internal set; }

        internal RplBounceEventArgs()
        {
             
        }
    }
}