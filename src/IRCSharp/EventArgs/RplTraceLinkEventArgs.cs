using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceLink
    public sealed class RplTraceLinkEventArgs : EventArgs 
    { 
        public string VersionDebug { get; internal set; }
        public string Destination { get; internal set; }
        public string NextServer { get; internal set; }
        public string ProtocolVersion { get; internal set; }
        public string UptimeSeconds { get; internal set; }
        public string BackStreamSendq { get; internal set; }

        internal RplTraceLinkEventArgs()
        {
             
        }
    }
}