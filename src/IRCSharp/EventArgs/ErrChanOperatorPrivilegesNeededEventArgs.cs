using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrChanOperatorPrivilegesNeeded
    public sealed class ErrChanOperatorPrivilegesNeededEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }

        internal ErrChanOperatorPrivilegesNeededEventArgs()
        {
             
        }
    }
}