using System;

namespace IRCSharp.Entities.Enums
{
    [Flags]
    public enum ChannelPrivilege
    {
        Normal = 1,
        Voice = 2,
        Operator = 4,
        Unknown = 8
    }
}
