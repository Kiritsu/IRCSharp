using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplLinks
    public sealed class RplLinksEventArgs : EventArgs 
    { 
        public string Mask { get; internal set; }
        public string Server { get; internal set; }
        public string HopCount { get; internal set; }
        public string ServerInfo { get; internal set; }

        internal RplLinksEventArgs()
        {
             
        }
    }
}