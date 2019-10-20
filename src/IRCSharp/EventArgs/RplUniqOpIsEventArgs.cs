using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplUniqOpIs
    public sealed class RplUniqOpIsEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string Nickname { get; internal set; }

        internal RplUniqOpIsEventArgs()
        {
             
        }
    }
}