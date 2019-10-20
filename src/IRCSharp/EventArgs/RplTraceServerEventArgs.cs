using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceServer
    public sealed class RplTraceServerEventArgs : EventArgs 
    { 
        public string Class { get; internal set; }
        public string IntS { get; internal set; }
        public string IntC { get; internal set; }
        public string Server { get; internal set; }
        public string Fullhost { get; internal set; }
        public string ProtocolVersion { get; internal set; }

        internal RplTraceServerEventArgs()
        {
             
        }
    }
}