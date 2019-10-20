using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplBanlist
    public sealed class RplBanlistEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string BanMask { get; internal set; }

        internal RplBanlistEventArgs()
        {
             
        }
    }
}