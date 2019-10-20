namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplWhoisUser
    public sealed class RplWhoisUserEventArgs : EventArgs 
    { 
        public string Nickname { get; internal set; }
        public string User { get; internal set; }
        public string Host { get; internal set; }
        public string RealName { get; internal set; }

        internal RplWhoisUserEventArgs()
        {
             
        }
    }
}