using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrBadChanMask
    public sealed class ErrBadChanMaskEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }

        internal ErrBadChanMaskEventArgs()
        {
             
        }
    }
}