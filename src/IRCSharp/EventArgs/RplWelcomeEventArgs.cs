using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWelcome
    public sealed class RplWelcomeEventArgs : EventArgs 
    { 
        public string Nick { get; internal set; }
        public string User { get; internal set; }
        public string Host { get; internal set; }

        internal RplWelcomeEventArgs()
        {
             
        }
    }
}