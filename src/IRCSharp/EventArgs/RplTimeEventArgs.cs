using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTime
    public sealed class RplTimeEventArgs : EventArgs 
    { 
        public string Server { get; internal set; }
        public string LocalServerTime { get; internal set; }

        internal RplTimeEventArgs()
        {
             
        }
    }
}