using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using IRCSharp.Entities;
using IRCSharp.Entities.Enums;
using IRCSharp.Entities.Models;
using IRCSharp.EventArgs;

namespace IRCSharp
{
    public sealed class IRCClient
    {
        private StreamWriter _writer;

        private readonly TcpClient _tcp;

        private readonly ConcurrentDictionary<string, User> _cachedUsers;
        private readonly ConcurrentDictionary<string, Channel> _cachedChannels;

        internal readonly IRCConfiguration _configuration;

        /// <summary>
        ///     Gets the cached <see cref="User"/>s.
        /// </summary>
        public ReadOnlyDictionary<string, User> CachedUsers { get; }

        /// <summary>
        ///     Gets the cached <see cref="Channel"/>s.
        /// </summary>
        public ReadOnlyDictionary<string, Channel> CachedChannels { get; }

        /// <summary>
        ///     Fires when data is received.
        /// </summary>
        public event Action<string> DataReceived;

        /// <summary>
        ///     Fires when the client is connected and authenticated to the remote server.
        /// </summary>
        public event Action<ReadyEventArgs> Ready;

        /// <summary>
        ///     Fires when a user has joined a channel.
        /// </summary>
        public event Action<UserJoinedEventArgs> UserJoined;

        /// <summary>
        ///     Fires when a user has left a channel.
        /// </summary>
        public event Action<UserLeftEventArgs> UserLeft;

        /// <summary>
        ///     Fires when a user quit.
        /// </summary>
        public event Action<UserQuitEventArgs> UserQuit;

        /// <summary>
        ///     Fires when a user quit.
        /// </summary>
        public event Action<NicknameChangedEventArgs> NicknameChanged;

        /// <summary>
        ///     Fires when a message is received.
        /// </summary>
        public event Action<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        ///     Fires when a notice is received.
        /// </summary>
        public event Action<NoticeReceivedEventArgs> NoticeReceived;

        /// <summary>
        ///     Fires when channel modes have been updated on a channel.
        /// </summary>
        public event Action<ChannelModesUpdatedEventArgs> ChannelModeUpdated;

        /// <summary>
        ///     Fires when a user has been kicked from a channel.
        /// </summary>
        public event Action<UserKickedEventArgs> UserKicked;

        /// <summary>
        ///     True when authenticated to the remote server.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        ///     Represents the current user.
        /// </summary>
        public User CurrentUser { get; private set; }

        /// <summary>
        ///     Creates a new <see cref="IRCClient"/>.
        /// </summary>
        public IRCClient(IRCConfiguration configuration)
        {
            DataReceived += OnDataReceived;

            _configuration = new IRCConfiguration(configuration);

            _tcp = new TcpClient();
            _cachedUsers = new ConcurrentDictionary<string, User>();
            _cachedChannels = new ConcurrentDictionary<string, Channel>();
        }

        /// <summary>
        ///     Connects to the remote server.
        /// </summary>
        public void Connect()
        {
            _tcp.Connect(_configuration.Hostname, _configuration.Port);
            _writer = new StreamWriter(_tcp.GetStream());

            //Sending authentication.
            if (!string.IsNullOrWhiteSpace(_configuration.Password))
            {
                Send($"PASS {_configuration.Password}");
            }

            Send($"NICK {_configuration.Username}");
            Send($"USER {_configuration.Identd} 0 * :{_configuration.RealName}");

            new Thread(() =>
            {
                using (var reader = new StreamReader(_tcp.GetStream()))
                {
                    while (_tcp.Connected)
                    {
                        var data = reader.ReadLine();

                        DataReceived?.Invoke(data);
                    }
                }
            }).Start();
        }

        private void OnDataReceived(string data)
        {
            if (data.StartsWith("PING"))
            {
                Send(data.Replace("PING", "PONG"));
            }

            var content = data.Split(' ');
            if (content.Length <= 1)
            {
                return;
            }

            if (content[0].StartsWith(":"))
            {
                var sender = content[0].Substring(1);
                if (!sender.Contains("!") && !sender.Contains("@"))
                {
                    _configuration.Host = sender;
                    HandleServerData(content.Skip(1).ToArray(), data);
                }
                else
                {
                    HandleUserData(sender, content.Skip(1).ToArray(), data);
                }
            }
        }

        private void HandleUserData(string sender, string[] content, string data)
        {
            var username = sender.Substring(0, sender.IndexOf('!'));
            if (!_cachedUsers.TryGetValue(username, out var user))
            {
                user = new User(this)
                {
                    Username = username
                };

                _cachedUsers.TryAdd(username, user);
            }

            switch (content[0])
            {
                case "JOIN":
                    {
                        if (!_cachedChannels.TryGetValue(content[1], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = content[1]
                            };

                            _cachedChannels.TryAdd(content[1], channel);
                        }

                        var channelUser = new ChannelUser(this, user, channel);
                        if (!channel._users.Any(x => x == user))
                        {
                            channel._users.Add(channelUser);
                        }

                        if (!user._channels.Any(x => x == channel))
                        {
                            user._channels.Add(channel);
                        }

                        UserJoined?.Invoke(new UserJoinedEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            Channel = channel,
                            User = channelUser
                        });

                        return;
                    }
                case "PART":
                    {
                        var channelName = content[1];
                        if (channelName.StartsWith(':'))
                        {
                            channelName = channelName.Substring(1);
                        }

                        if (!_cachedChannels.TryGetValue(channelName, out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = channelName
                            };

                            _cachedChannels.TryAdd(channelName, channel);
                        }

                        if (user._channels.Any(x => x == channel))
                        {
                            user._channels.Remove(channel);
                        }

                        if (channel._users.Any(x => x == user))
                        {
                            channel._users.RemoveAll(x => x == user);
                        }

                        UserLeft?.Invoke(new UserLeftEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            Channel = channel,
                            User = user,
                            Reason = content.Length > 2 ? data.Substring(data.IndexOf(':') + 1) : ""
                        });

                        return;
                    }
                case "QUIT":
                    {
                        _cachedUsers.TryRemove(user.Username, out _);

                        foreach (var channel in _cachedChannels.Values)
                        {
                            channel._users.RemoveAll(x => x == user);
                        }

                        var reason = "";
                        var idof = data.IndexOf(':');
                        if (idof != -1)
                        {
                            reason = data.Substring(idof + 1);
                        }

                        UserQuit?.Invoke(new UserQuitEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            User = user,
                            Reason = reason
                        });

                        return;
                    }
                case "NICK":
                    {
                        _cachedUsers.TryRemove(user.Username, out var oldUser);
                        user.Username = content[1].Substring(1);
                        _cachedUsers.TryRemove(user.Username, out _);
                        _cachedUsers.TryAdd(user.Username, user);

                        NicknameChanged?.Invoke(new NicknameChangedEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            User = user,
                            OldUsername = oldUser.Username,
                            NewUsername = user.Username
                        });

                        return;
                    }
                case "PRIVMSG":
                    {
                        var message = data.Substring(data.IndexOf(':') + 1);
                        if (!user._channelMessages.TryGetValue(content[1], out var list))
                        {
                            list = new List<string> { message };
                            user._channelMessages.TryAdd(content[1], list);
                        }
                        else
                        {
                            list.Add(message);
                        }

                        if (content[1][0] == '#')
                        {
                            if (!_cachedChannels.TryGetValue(content[1], out var channel))
                            {
                                channel = new Channel(this)
                                {
                                    Name = content[1]
                                };

                                _cachedChannels.TryAdd(content[1], channel);
                            }

                            channel._messages.Add((user, message));

                            MessageReceived?.Invoke(new MessageReceivedEventArgs
                            {
                                Client = this,
                                Channel = channel,
                                CurrentUser = CurrentUser,
                                Message = message,
                                User = channel.Users.FirstOrDefault(x => x == user) ?? user
                            });

                            return;
                        }

                        MessageReceived?.Invoke(new MessageReceivedEventArgs
                        {
                            Client = this,
                            Channel = null,
                            CurrentUser = CurrentUser,
                            Message = message,
                            User = user
                        });

                        return;
                    }
                case "NOTICE":
                    {
                        var message = data.Substring(data.IndexOf(':') + 1);
                        if (!user._channelMessages.TryGetValue(content[1], out var list))
                        {
                            list = new List<string> { message };
                            user._channelMessages.TryAdd(content[1], list);
                        }
                        else
                        {
                            list.Add(message);
                        }

                        NoticeReceived?.Invoke(new NoticeReceivedEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            Message = message,
                            User = user
                        });

                        return;
                    }
                case "MODE":
                    {
                        if (!_cachedChannels.TryGetValue(content[1], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = content[1]
                            };

                            _cachedChannels.TryAdd(content[1], channel);
                        }

                        var iofplus = content[2].IndexOf('+');
                        var iofminus = content[2].IndexOf('-');

                        var modesPlus = Array.Empty<char>();
                        var modesMinus = Array.Empty<char>();

                        if (iofplus != -1)
                        {
                            modesPlus = content[2].Substring(iofplus + 1).ToCharArray();
                        }
                        if (iofminus != -1)
                        {
                            modesMinus = iofplus != -1
                                ? content[2].Substring(iofminus + 1, content[2].Length - iofplus - 1).ToCharArray()
                                : content[2].Substring(iofminus + 1).ToCharArray();
                        }

                        for (var i = 0; i < modesPlus.Length; i++)
                        {
                            if (!channel._modes.Contains(modesPlus[i]))
                            {
                                channel._modes.Add(modesPlus[i]);
                            }
                        }

                        for (var i = 0; i < modesMinus.Length; i++)
                        {
                            if (!channel._modes.Contains(modesMinus[i]))
                            {
                                channel._modes.Add(modesMinus[i]);
                            }
                        }

                        var complexModes = modesMinus.Where(x => _configuration.ComplexChanModes.Any(y => y == x && x != 'l')).ToList();
                        complexModes.AddRange(modesPlus.Where(x => _configuration.ComplexChanModes.Any(y => y == x)));

                        var extraArgs = content.Skip(3).ToList();

                        var modesArgs = new Dictionary<char, (char, string)>();
                        for (var i = 0; i < complexModes.Count && i < extraArgs.Count; i++)
                        {
                            modesArgs.Add(complexModes[i], (modesMinus.Contains(complexModes[i]) ? '-' : '+', extraArgs[i]));
                        }

                        foreach (var complexMod in modesArgs)
                        {
                            switch (complexMod.Key)
                            {
                                case 'b':
                                    if (complexMod.Value.Item1 == '-')
                                    {
                                        channel._banList.Remove(complexMod.Value.Item2);
                                    }
                                    else
                                    {
                                        channel._banList.Add(complexMod.Value.Item2);
                                    }
                                    break;
                                case 'l':
                                    channel.Limit = int.Parse(complexMod.Value.Item2);
                                    break;
                                case 'k':
                                    channel.Key = complexMod.Value.Item1 == '+' ? complexMod.Value.Item2 : "";
                                    break;
                            }
                        }

                        ChannelModeUpdated?.Invoke(new ChannelModesUpdatedEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            Channel = channel,
                            ModesAdded = new ReadOnlyCollection<char>(modesPlus),
                            ModesRemoved = new ReadOnlyCollection<char>(modesMinus),
                            ModesArgs = new ReadOnlyDictionary<char, (char, string)>(modesArgs),
                            User = channel.Users.FirstOrDefault(x => x == user)
                        });

                        return;
                    }
                case "KICK":
                    {
                        if (!_cachedChannels.TryGetValue(content[1], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = content[1]
                            };

                            _cachedChannels.TryAdd(content[1], channel);
                        }

                        if (!_cachedUsers.TryGetValue(content[2], out var kicked))
                        {
                            kicked = new User(this)
                            {
                                Username = content[2]
                            };

                            _cachedUsers.TryAdd(content[2], kicked);
                        }

                        var kicker = channel._users.FirstOrDefault(x => x == user);
                        if (kicker is null)
                        {
                            kicker = new ChannelUser(this, user, channel);
                            channel._users.Add(kicker);
                        }

                        kicked._channels.Remove(channel);
                        channel._users.RemoveAll(x => x == kicked);

                        UserKicked?.Invoke(new UserKickedEventArgs
                        {
                            Client = this,
                            CurrentUser = CurrentUser,
                            Channel = channel,
                            Kicked = kicked,
                            Kicker = kicker,
                            Reason = content.Length > 2 ? data.Substring(data.IndexOf(':') + 1) : ""
                        });

                        return;
                    }
            }
        }

        private void HandleServerData(string[] data, string raw)
        {
            var command = data[0];
            var username = data[1];
            var content = raw.Substring(raw.IndexOf(':') + 1);

            if (!int.TryParse(command, out var code))
            {
                return;
            }

            switch (code)
            {
                case 1:
                    Connected = true;

                    CurrentUser = new User(this)
                    {
                        Username = username,
                    };

                    _cachedUsers.TryAdd(username, CurrentUser);

                    Ready?.Invoke(new ReadyEventArgs
                    {
                        Client = this,
                        CurrentUser = CurrentUser
                    });

                    Send($"WHOIS {_configuration.Host ?? username} {username}");
                    break;

                case 5:
                    var chanModes = data.FirstOrDefault(x => x.StartsWith("CHANMODES=")).Split('=')[1];
                    var complexChanModes = chanModes.Split(',').Where(x => x.Length == 1);
                    _configuration.ComplexChanModes = complexChanModes.Select(x => x[0]).ToArray();
                    break;

                case 311:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user.Username = data[2];
                        user.Identd = data[3];
                        user.Host = data[4];
                        user.Realname = content;

                        return;
                    }

                case 312:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user.Server = data[3];

                        return;
                    }

                case 313:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user.IRCOperator = true;

                        return;
                    }

                case 317:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user.Idle = TimeSpan.FromSeconds(int.Parse(data[3]));
                        user.Signon = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data[4]));

                        return;
                    }

                case 338:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user.ReverseIp = data[3];
                        user.Ip = data[4];

                        return;
                    }

                case 332:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        channel.Topic = new Topic
                        {
                            Content = content
                        };

                        return;
                    }

                case 333:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        channel.Topic.Author = data[3];
                        channel.Topic.ModifiedAt = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data[4]));

                        return;
                    }

                case 353:
                    {
                        if (!_cachedChannels.TryGetValue(data[3], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = data[3]
                            };

                            _cachedChannels.TryAdd(data[3], channel);
                        }

                        var users = content.Split(' ');
                        foreach (var u in users)
                        {
                            var name = u.Substring(1);
                            if (!_cachedUsers.TryGetValue(name, out var user))
                            {
                                user = new User(this) { Username = name };
                                _cachedUsers.TryAdd(name, user);
                            }

                            if (!user._channels.Contains(channel))
                            {
                                user._channels.Add(channel);
                            }

                            var channelUser = new ChannelUser(this, user, channel);

                            switch (u[0])
                            {
                                case '+':
                                    channelUser.Privileges = ChannelPrivilege.Voice;
                                    break;
                                case '@':
                                    channelUser.Privileges = ChannelPrivilege.Operator;
                                    break;
                                default:
                                    channelUser.Privileges = ChannelPrivilege.Unknown;
                                    break;
                            }

                            if (!channel._users.Contains(channelUser))
                            {
                                channel._users.Add(channelUser);
                            }
                        }

                        return;
                    }

                case 324:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        if (data[3].Length > 1)
                        {
                            channel._modes.Clear();
                            channel._modes.AddRange(data[3].Substring(1));
                        }

                        return;
                    }

                case 329:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel(this)
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        channel.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data[3]));
                        return;
                    }

                case 319:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User(this) { Username = data[2] };
                            _cachedUsers.TryAdd(data[2], user);
                        }

                        user._channels.Clear();
                        var channels = content.Split(' ');
                        foreach (var chan in channels)
                        {
                            var name = chan.Substring(chan.IndexOf("#"));
                            if (!_cachedChannels.TryGetValue(name, out var channel))
                            {
                                channel = new Channel(this)
                                {
                                    Name = name
                                };

                                _cachedChannels.TryAdd(name, channel);
                            }

                            ChannelUser channelUser;
                            if (!channel._users.Contains(user))
                            {
                                channelUser = new ChannelUser(this, user, channel);
                                channel._users.Add(channelUser);
                            }
                            else
                            {
                                channelUser = channel._users.FirstOrDefault(x => x == user);
                            }

                            if (!user._channels.Contains(channel))
                            {
                                user._channels.Add(channel);
                            }

                            switch (chan[0])
                            {
                                case '#':
                                    channelUser.Privileges = ChannelPrivilege.Normal;
                                    break;
                                case '@':
                                    channelUser.Privileges = ChannelPrivilege.Operator;
                                    break;
                                case '+':
                                    channelUser.Privileges = ChannelPrivilege.Voice;
                                    break;
                                default:
                                    channelUser.Privileges = ChannelPrivilege.Unknown;
                                    break;
                            }
                        }

                        return;
                    }
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="string"/> to the server.
        /// </summary>
        /// <param name="data">Content to send.</param>
        public void Send(string data)
        {
            _writer.WriteLine(data);
            _writer.Flush();
        }
    }
}
