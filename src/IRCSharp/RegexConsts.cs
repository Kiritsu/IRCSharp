using System.Text.RegularExpressions;

namespace IRCSharp
{
    public class RegexConsts
    {
        //todo: ensure every RPL has its proper event and handle those who needs s p e c i a l b e h a v i o r 
        
        //001
        public static readonly Regex RPL_WELCOME = new Regex(
            "^Welcome to the Internet Relay Network (?<nick>.+)!(?<user>.+)@(?<host>.+)$", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //002
        public static readonly Regex RPL_YOURHOST = new Regex(
            "^Your host is (?<servername>.+), running version (?<version>.+)$", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //003
        public static readonly Regex RPL_CREATED = new Regex(
            "^This server was created (?<date>.+)$", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //004
        public static readonly Regex RPL_MYINFO = new Regex(
            "^(?<server_name>[^\\s]+) (?<version>[^\\s]+) (?<user_modes>[^\\s]+) (?<channel_modes>[^\\s]+)$", 
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //005
        public static readonly Regex RPL_BOUNCE = new Regex(
            "^Try server (?<server_name>.+), port (?<port>.+)$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        /* wtf???
         * <- :OGN5.OnlineGamesNet.net 005 Kiritsu WHOX WALLCHOPS WALLVOICES USERIP CPRIVMSG CNOTICE SILENCE=15 MODES=6 MAXCHANNELS=30 MAXBANS=99 NICKLEN=30 :are supported by this server
         * <- :OGN5.OnlineGamesNet.net 005 Kiritsu MAXNICKLEN=30 TOPICLEN=500 AWAYLEN=300 KICKLEN=500 CHANNELLEN=75 MAXCHANNELLEN=200 CHANTYPES=# PREFIX=(ov)@+ STATUSMSG=@+ CHANMODES=b,k,l,cCimMnpstrDdRz CASEMAPPING=rfc1459 NETWORK=OnlineGamesNet :are supported by this server
         */
        
        //301
        public static readonly Regex RPL_AWAY = new Regex(
            "^(?<nick>.+) :(?<message>.+)$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //302
        public static readonly Regex RPL_USERHOST = new Regex(
            "^:*1(?<reply>.+) \\*\\(*$", //don't need to get ' " " <reply> )'
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //303
        public static readonly Regex RPL_ISON = new Regex(
            "^:*1(?<nick>.+) \\*\\(*$", //don't need to get ' " " <nick> )'
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //305 RPL_UNAWAY :You are no longer marked as being away
        //306 RPL_NOWAWAY :You have been marked as being away
        
        //311 RPL_WHOISUSER manually handled
        //312 RPL_WHOISSERVER manually handled
        //313 RPL_WHOISOPERATOR manually handled
        //317 RPL_WHOISIDLE manually handled
        //318 RPL_ENDOFWHOIS useless to handle
        //319 RPL_WHOISCHANNELS manually handled
        
        //todo: 314 RPL_WHOWASUSER need to handle 
        //369 RPL_ENDOFWHOWAS useless to handle
        
        //321 RPL_LISTSTART obsolete
        //todo: 322 RPL_LIST need to handle "<#channel> <# visible> :<topic>" or "<#channel> <user_count> :<topic>" 
        //323 RPL_LISTEND useless to handle
        
        //todo: 325 RPL_UNIQOPIS need to handle "<#channel> <nickname>" 
        //324 RPL_CHANNELMODEIS manually handled
        
        //331 RPL_NOTOPIC useless to handle
        //332 RPL_TOPIC manually handled

        //todo: 341 RPL_INVITING need to handle "<channel> <nick>"  
        //todo: 342 RPL_SUMMONING need to handle "<user> :Summoning user to IRC"
        
        //todo: 346 RPL_INVITELIST need to handle "<channel> <invite_mask>"
        //347 RPL_ENDOFINVITELIST useless to handle
        //todo: 348 RPL_EXCEPTLIST need to handle "<channel> <exceptionmask>"
        //349 RPL_ENDOFEXCEPTLIST useless to handle
        //todo: 351 RPL_VERSION need to handle "<version>.<debuglevel> <server> :<comment>"
        public static readonly Regex RPL_VERSION = new Regex(
            "^(?<version>[^\\s]+)\\.(?<debug_level>[^\\s]+) (?<server>[^\\s]+) :(?<comments>.+)$",
            RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        
        //todo: 352 RPL_WHOREPLY need to handle "<channel> <user> <host> <server> <nick> <wtf_no_one_cares_of_this> :<hop_count> <real name>"
        //315 RPL_ENDOFWHO useless to handle
        
        //353 RPL_NAMREPLY manually handled
        //366 RPL_ENDOFNAMES useless to handle
        
        //todo: 364 RPL_LINKS need to handle "<mask> <server> :<hop_count> <server_info>"
        public static readonly Regex RPL_LINKS = new Regex(
            "^(?<mask>[^\\s]+) (?<server>[^\\s]+) :(?<hop_count>[^\\s]+) (?<server_info>[^\\s]+) $",
        RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        //365 RPL_ENDOFLINKS useless to handle
        
        //367 RPL_BANLIST manually handled
        //368 RPL_ENDOFBANLIST useless to handle
        
        //todo: 371 RPL_INFO ":<text>"
        //374 RPL_ENDOFINFO useless to handle
        
        //todo: 375 RPL_MOTDSTART need to handle ":- <server> Message of the day - "
        //todo: 372 RPL_MOTD need to handle ":- <text>"
        //376 RPL_ENDOFMOTD useless to handle
        
        //todo: 381 RPL_YOUREOPER need to handle
        
        //todo: 382 TPL_REHASHING need to handle "<config_file> :Rehashing"
        
        //todo: 383 RPL_YOURESERVICE need to handle "Your are service <service_name>"
        
        //todo: 391 RPL_TIME need to handle "<server> :<local server's time>"
        
        //todo: 392 RPL_USERSSTART need to handle
        //todo: 393 RPL_USERS need to handle ":<username> <ttyline> <hostname>"
        //394 RPL_ENDOFUSERS useless to handle
        //todo: 395 RPL_NOUSERS need to handle 
    }
}