using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrBadMask
    public sealed class ErrBadMaskEventArgs : EventArgs 
    { 
        public string Mask { get; internal set; }

        internal ErrBadMaskEventArgs()
        {
             
        }
    }
}