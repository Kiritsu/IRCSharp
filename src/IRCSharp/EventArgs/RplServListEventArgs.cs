using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplServList
    public sealed class RplServListEventArgs : EventArgs 
    { 
        public string Name { get; internal set; }
        public string Server { get; internal set; }
        public string Mask { get; internal set; }
        public string Type { get; internal set; }
        public string HopCount { get; internal set; }
        public string Info { get; internal set; }

        internal RplServListEventArgs()
        {
             
        }
    }
}