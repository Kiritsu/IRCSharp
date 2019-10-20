namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplTooManyTargets
    public sealed class RplTooManyTargetsEventArgs : EventArgs 
    { 
        public string Target { get; internal set; }
        public string ErrorCode { get; internal set; }
        public string AbortMessage { get; internal set; }

        internal RplTooManyTargetsEventArgs()
        {
             
        }
    }
}