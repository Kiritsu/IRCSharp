using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrUnknownCommand
    public sealed class ErrUnknownCommandEventArgs : EventArgs 
    { 
        public string Command { get; internal set; }

        internal ErrUnknownCommandEventArgs()
        {
             
        }
    }
}