using System;
using IRCSharp;
using IRCSharp.Entities;
using IRCSharp.Services;

namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrFileError
    public sealed class ErrFileErrorEventArgs : EventArgs 
    { 
        public string FileOp { get; internal set; }
        public string File { get; internal set; }

        internal ErrFileErrorEventArgs()
        {
             
        }
    }
}