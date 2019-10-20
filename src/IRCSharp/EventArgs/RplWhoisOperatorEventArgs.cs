using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoisOperator
    public sealed class RplWhoisOperatorEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }

        internal RplWhoisOperatorEventArgs()
        {
             
        }
    }
}