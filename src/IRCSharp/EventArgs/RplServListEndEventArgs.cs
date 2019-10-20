using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplServListEnd
    public sealed class RplServListEndEventArgs : EventArgs 
    { 
        public string Mask { get; internal set; }
        public string Type { get; internal set; }

        internal RplServListEndEventArgs()
        {
             
        }
    }
}