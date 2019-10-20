namespace IRCSharp.EventArgs
{
    //Auto-Generated code for ErrUnknownMode
    public sealed class ErrUnknownModeEventArgs : EventArgs 
    { 
        public string Mode { get; internal set; }
        public string Channel { get; internal set; }

        internal ErrUnknownModeEventArgs()
        {
             
        }
    }
}