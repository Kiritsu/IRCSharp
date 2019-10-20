using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplYourHost
    public sealed class RplYourHostEventArgs : EventArgs 
    { 
        public string ServerName { get; internal set; }
        public string Version { get; internal set; }

        internal RplYourHostEventArgs()
        {
             
        }
    }
}