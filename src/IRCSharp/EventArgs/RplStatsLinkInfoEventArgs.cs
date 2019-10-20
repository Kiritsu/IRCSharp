namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplStatsLinkInfo
    public sealed class RplStatsLinkInfoEventArgs : EventArgs 
    { 
        public string Linkname { get; internal set; }
        public string SendQ { get; internal set; }
        public string SendMessages { get; internal set; }
        public string SentKBytes { get; internal set; }
        public string ReceivedMessages { get; internal set; }
        public string ReceviedKBytes { get; internal set; }
        public string TimeOpen { get; internal set; }

        internal RplStatsLinkInfoEventArgs()
        {
             
        }
    }
}