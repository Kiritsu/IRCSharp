using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoReply
    public sealed class RplWhoReplyEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string User { get; internal set; }
        public string Host { get; internal set; }
        public string Server { get; internal set; }
        public string Args { get; internal set; }
        public string HopCount { get; internal set; }
        public string RealName { get; internal set; }

        internal RplWhoReplyEventArgs()
        {
             
        }
    }
}