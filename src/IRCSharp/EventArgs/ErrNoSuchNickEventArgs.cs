using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNoSuchNick
    public sealed class ErrNoSuchNickEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }

        internal ErrNoSuchNickEventArgs()
        {
             
        }
    }
}