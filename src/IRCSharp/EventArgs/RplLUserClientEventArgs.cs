using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplLUserClient
    public sealed class RplLUserClientEventArgs : EventArgs 
    { 
        public string UserCount { get; internal set; }
        public string ServiceCount { get; internal set; }
        public string ServerCount { get; internal set; }

        internal RplLUserClientEventArgs()
        {
             
        }
    }
}