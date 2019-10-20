namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplStatsCommands
    public sealed class RplStatsCommandsEventArgs : EventArgs 
    { 
        public string Command { get; internal set; }
        public string Count { get; internal set; }
        public string ByteCount { get; internal set; }
        public string RemoteCount { get; internal set; }

        internal RplStatsCommandsEventArgs()
        {
             
        }
    }
}