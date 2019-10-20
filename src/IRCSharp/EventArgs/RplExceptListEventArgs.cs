using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplExceptList
    public sealed class RplExceptListEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string ExceptionMask { get; internal set; }

        internal RplExceptListEventArgs()
        {
             
        }
    }
}