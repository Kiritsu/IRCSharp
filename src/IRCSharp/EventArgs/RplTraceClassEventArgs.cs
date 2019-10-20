using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceClass
    public sealed class RplTraceClassEventArgs : EventArgs 
    { 
        public string Class { get; internal set; }
        public string Count { get; internal set; }

        internal RplTraceClassEventArgs()
        {
             
        }
    }
}