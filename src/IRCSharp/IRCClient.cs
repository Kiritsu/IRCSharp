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
                                Name = content[1]
                            };

                            _cachedChannels.TryAdd(content[1], channel);
                        }

                        if (user._channels.Any(x => x == channel))
                        {
                            user._channels.Remove(channel);
                        }

                        if (channel._users.Any(x => x == user))
                        {
                            channel._users.Remove(channel._users.FirstOrDefault(x => x == user));
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
                        _cachedUsers.TryRemove(username, out _);

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
                case "PRIVMSG": //<- :Soronax!Soronax@Soronax.gameadmin.NosTaleFr PRIVMSG Kiritsu :ccccc
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

                        return;
                    }
                case "NOTICE": //<- :Soronax!Soronax@Soronax.gameadmin.NosTaleFr NOTICE Kiritsu :cc
                    {
                        return;
                    }
                case "MODE":
                    {
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
