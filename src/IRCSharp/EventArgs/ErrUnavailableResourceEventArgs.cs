using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrUnavailableResource
    public sealed class ErrUnavailableResourceEventArgs : EventArgs 
    { 
        public string Ressource { get; internal set; }

        internal ErrUnavailableResourceEventArgs()
        {
             
        }
    }
}