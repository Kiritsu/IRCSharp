using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplUsers
    public sealed class RplUsersEventArgs : EventArgs 
    { 
        public string Username { get; internal set; }
        public string TtyLine { get; internal set; }
        public string Hostname { get; internal set; }

        internal RplUsersEventArgs()
        {
             
        }
    }
}