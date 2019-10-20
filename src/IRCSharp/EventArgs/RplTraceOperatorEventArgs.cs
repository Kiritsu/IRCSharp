using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTraceOperator
    public sealed class RplTraceOperatorEventArgs : EventArgs 
    { 
        public string Class { get; internal set; }
        public string Server { get; internal set; }

        internal RplTraceOperatorEventArgs()
        {
             
        }
    }
}