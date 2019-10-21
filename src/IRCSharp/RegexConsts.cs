using System.Text.RegularExpressions;

namespace IRCSharp
{
    public class RegexConsts
    {
        private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase;
        
        //todo: ensure every RPL has its proper event and handle those who needs s p e c i a l b e h a v i o r 
        
        //001
        public static readonly Regex RPL_WELCOME = new Regex(
            "^Welcome to the Internet Relay Network (?<nick>.+)!(?<user>.+)@(?<host>.+)$", 
            Options);
        
        //002
        public static readonly Regex RPL_YOUR_HOST = new Regex(
            "^Your host is (?<server_name>.+), running version (?<version>.+)$", 
            Options);
        
        //003
        public static readonly Regex RPL_CREATED = new Regex(
            "^This server was created (?<date>.+)$", 
            Options);
        
        //004
        public static readonly Regex RPL_MY_INFO = new Regex(
            "^(?<server_name>[^\\s]+) (?<version>[^\\s]+) (?<user_modes>[^\\s]+) (?<channel_modes>[^\\s]+) (?<unknown>.*)$", 
            Options);
        
        //005
        public static readonly Regex RPL_BOUNCE = new Regex(
            "^Try server (?<server_name>.+), port (?<port>.+)$",
            Options);
        /* wtf???
         * <- :OGN5.OnlineGamesNet.net 005 Kiritsu WHOX WALLCHOPS WALLVOICES USERIP CPRIVMSG CNOTICE SILENCE=15 MODES=6 MAXCHANNELS=30 MAXBANS=99 NICKLEN=30 :are supported by this server
         * <- :OGN5.OnlineGamesNet.net 005 Kiritsu MAXNICKLEN=30 TOPICLEN=500 AWAYLEN=300 KICKLEN=500 CHANNELLEN=75 MAXCHANNELLEN=200 CHANTYPES=# PREFIX=(ov)@+ STATUSMSG=@+ CHANMODES=b,k,l,cCimMnpstrDdRz CASEMAPPING=rfc1459 NETWORK=OnlineGamesNet :are supported by this server
         */
        
        //200
        public static readonly Regex RPL_TRACE_LINK = new Regex(
            "^Link (?<version_debug>[^\\s]+) (?<destination>[^\\s]+) (?<next_server>[^\\s]+) (?<protocol_version>[^\\s]+) (?<uptime_seconds>[^\\s]+) (?<back_stream_sendQ>[^\\s]+) $",
            Options);
        
        //201
        public static readonly Regex RPL_TRACE_CONNECTING = new Regex(
            "^Try. (?<class>.+) (?<server>.+)$", 
            Options);
        
        //202
        public static readonly Regex RPL_TRACE_HANDSHAKE = new Regex(
            "^H.S. (?<class>.+) (?<server>.+)$", 
            Options);
        
        //203
        public static readonly Regex RPL_TRACE_UNKNOWN = new Regex(
            "^\\?\\?\\?\\? (?<class>.+) (?<ip>.+)$",
            Options);
        
        //204
        public static readonly Regex RPL_TRACE_OPERATOR = new Regex(
            "^Oper (?<class>.+) (?<server>.+)$", 
            Options);
        
        //205
        public static readonly Regex RPL_TRACE_USER = new Regex(
            "^User (?<class>.+) (?<server>.+)$", 
            Options);
        
        //206
        public static readonly Regex RPL_TRACE_SERVER = new Regex(
            "^Serv (?<class>.+) (?<int_s>.+)S (?<int_c>.+)C (?<server>.+) (?<fullhost>.+) V(?<protocol_version>.+) $",
            Options);
        
        //207
        public static readonly Regex RPL_TRACE_SERVICE = new Regex(
            "^Service (?<class>.+) (?<name>.+) (?<type>.+) (?<active_type>.+)$",
            Options);
        
        //208 
        public static readonly Regex RPL_TRACE_NEWTYPE = new Regex(
            "^(?<new_type>.+) 0 (?<client_name>.+)$",
            Options);
        
        //209
        public static readonly Regex RPL_TRACE_CLASS = new Regex(
            "^Class (?<class>.+) (?<count>.+)$",
            Options);
        
        //261
        public static readonly Regex RPL_TRACE_LOG = new Regex(
            "^File (?<log_file>.+) (?<debug_level>.+)$",
            Options);
        
        //262 RPL_TRACEEND useless to handle

        //todo: 301 need to handle
        public static readonly Regex RPL_AWAY = new Regex(
            "^(?<nick>.+) :(?<message>.+)$",
            Options);
        
        //302
        public static readonly Regex RPL_USER_HOST = new Regex(
            "^:*1(?<reply>.+) \\*\\(*$", //don't need to get ' " " <reply> )'
            Options);
        
        //303
        public static readonly Regex RPL_IS_ON = new Regex(
            "^:*1(?<nick>.+) \\*\\(*$", //don't need to get ' " " <nick> )'
            Options);
        
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
            Options);
        
        //todo: 352 RPL_WHOREPLY need to handle "<channel> <user> <host> <server> <nick> <wtf_no_one_cares_of_this> :<hop_count> <real name>"
        //315 RPL_ENDOFWHO useless to handle
        
        //353 RPL_NAMREPLY manually handled
        //366 RPL_ENDOFNAMES useless to handle
        
        //todo: 364 RPL_LINKS need to handle "<mask> <server> :<hop_count> <server_info>"
        public static readonly Regex RPL_LINKS = new Regex(
            "^(?<mask>[^\\s]+) (?<server>[^\\s]+) :(?<hop_count>[^\\s]+) (?<server_info>[^\\s]+) $",
        Options);
        
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
        
        //407
        public static readonly Regex RPL_TOO_MANY_TARGETS = new Regex(
            "^(?<target>.+) :(?<error_code>.+) recipients\\. (?<abort_message>.+)$",
            Options);
        
        //436
        public static readonly Regex ERR_NICK_COLLISION = new Regex(
            "^(?<nickname>.+) :Nickname collision KILL from (?<user>.+)@(?<host>.+)$",
            Options);
    }
}