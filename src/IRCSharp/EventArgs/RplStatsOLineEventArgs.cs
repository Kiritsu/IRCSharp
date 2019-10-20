using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplStatsOLine
    public sealed class RplStatsOLineEventArgs : EventArgs 
    { 
        public string Hostmask { get; internal set; }
        public string Name { get; internal set; }

        internal RplStatsOLineEventArgs()
        {
             
        }
    }
}