namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoisChannels
    public sealed class RplWhoisChannelsEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string Channels { get; internal set; }

        internal RplWhoisChannelsEventArgs()
        {
             
        }
    }
}