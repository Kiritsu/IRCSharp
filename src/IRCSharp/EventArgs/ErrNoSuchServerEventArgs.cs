using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNoSuchServer
    public sealed class ErrNoSuchServerEventArgs : EventArgs 
    { 
        public string ServerName { get; internal set; }

        internal ErrNoSuchServerEventArgs()
        {
             
        }
    }
}