using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplMotdStart
    public sealed class RplMotdStartEventArgs : EventArgs 
    { 
        public string Server { get; internal set; }

        internal RplMotdStartEventArgs()
        {
             
        }
    }
}