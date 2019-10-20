using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceUnknown
    public sealed class RplTraceUnknownEventArgs : EventArgs 
    { 
        public string Class { get; internal set; }
        public string Ip { get; internal set; }

        internal RplTraceUnknownEventArgs()
        {
             
        }
    }
}