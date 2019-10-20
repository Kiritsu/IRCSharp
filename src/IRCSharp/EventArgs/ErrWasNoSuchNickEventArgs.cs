using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrWasNoSuchNick
    public sealed class ErrWasNoSuchNickEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }

        internal ErrWasNoSuchNickEventArgs()
        {
             
        }
    }
}