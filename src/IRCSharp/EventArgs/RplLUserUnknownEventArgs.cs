using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplLUserUnknown
    public sealed class RplLUserUnknownEventArgs : EventArgs 
    { 
        public string UnknownConnections { get; internal set; }

        internal RplLUserUnknownEventArgs()
        {
             
        }
    }
}