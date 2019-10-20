using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplVersion
    public sealed class RplVersionEventArgs : EventArgs 
    { 
        public string Version { get; internal set; }
        public string DebugLevel { get; internal set; }
        public string Server { get; internal set; }
        public string Comment { get; internal set; }

        internal RplVersionEventArgs()
        {
             
        }
    }
}