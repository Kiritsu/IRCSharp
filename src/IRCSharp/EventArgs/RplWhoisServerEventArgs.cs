namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoisServer
    public sealed class RplWhoisServerEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string Server { get; internal set; }
        public string ServerInfo { get; internal set; }

        internal RplWhoisServerEventArgs()
        {
             
        }
    }
}