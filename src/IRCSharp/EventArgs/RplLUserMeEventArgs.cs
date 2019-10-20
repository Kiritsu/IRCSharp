using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplLUserMe
    public sealed class RplLUserMeEventArgs : EventArgs 
    { 
        public string ClientCount { get; internal set; }
        public string ServerCount { get; internal set; }

        internal RplLUserMeEventArgs()
        {
             
        }
    }
}