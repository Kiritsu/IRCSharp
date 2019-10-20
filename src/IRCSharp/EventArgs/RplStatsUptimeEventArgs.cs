using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplStatsUptime
    public sealed class RplStatsUptimeEventArgs : EventArgs 
    { 
        public string Days { get; internal set; }
        public string Hour { get; internal set; }

        internal RplStatsUptimeEventArgs()
        {
             
        }
    }
}