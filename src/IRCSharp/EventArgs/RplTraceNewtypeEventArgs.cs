using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceNewtype
    public sealed class RplTraceNewtypeEventArgs : EventArgs 
    { 
        public string NewType { get; internal set; }
        public string ClientName { get; internal set; }

        internal RplTraceNewtypeEventArgs()
        {
             
        }
    }
}