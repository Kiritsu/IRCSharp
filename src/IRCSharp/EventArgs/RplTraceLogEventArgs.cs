using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceLog
    public sealed class RplTraceLogEventArgs : EventArgs 
    { 
        public string LogFile { get; internal set; }
        public string DebugLevel { get; internal set; }

        internal RplTraceLogEventArgs()
        {
             
        }
    }
}