using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrErroneousNickname
    public sealed class ErrErroneousNicknameEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }

        internal ErrErroneousNicknameEventArgs()
        {
             
        }
    }
}