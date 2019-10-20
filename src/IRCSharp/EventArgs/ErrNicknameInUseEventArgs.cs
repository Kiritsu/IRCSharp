using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNicknameInUse
    public sealed class ErrNicknameInUseEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }

        internal ErrNicknameInUseEventArgs()
        {
             
        }
    }
}