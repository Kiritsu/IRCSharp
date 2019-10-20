namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplChannelModeIs
    public sealed class RplChannelModeIsEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string Mode { get; internal set; }
        public string ModeParams { get; internal set; }

        internal RplChannelModeIsEventArgs()
        {
             
        }
    }
}