using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrNeedMoreParams
    public sealed class ErrNeedMoreParamsEventArgs : EventArgs 
    { 
        public string Command { get; internal set; }

        internal ErrNeedMoreParamsEventArgs()
        {
             
        }
    }
}