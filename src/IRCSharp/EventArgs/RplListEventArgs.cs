using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplList
    public sealed class RplListEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string UserCount { get; internal set; }
        public string Topic { get; internal set; }

        internal RplListEventArgs()
        {
             
        }
    }
}