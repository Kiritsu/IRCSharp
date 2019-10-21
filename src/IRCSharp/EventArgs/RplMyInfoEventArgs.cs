namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplMyInfo
    public sealed class RplMyInfoEventArgs : EventArgs 
    { 
        public string ServerName { get; internal set; }
        public string Version { get; internal set; }
        public string UserModes { get; internal set; }
        public string ChannelModes { get; internal set; }
        public string UnsupportedModes { get; internal set; }

        internal RplMyInfoEventArgs()
        {
             
        }
    }
}