using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNickCollision
    public sealed class ErrNickCollisionEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string User { get; internal set; }
        public string Host { get; internal set; }

        internal ErrNickCollisionEventArgs()
        {
             
        }
    }
}