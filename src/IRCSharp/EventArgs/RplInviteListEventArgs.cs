namespace IRCSharp.EventArgs
{
    //Auto-Generated code for RplInviteList
    public sealed class RplInviteListEventArgs : EventArgs 
    { 
        public string Channel { get; internal set; }
        public string InviteMask { get; internal set; }

        internal RplInviteListEventArgs()
        {
             
        }
    }
}