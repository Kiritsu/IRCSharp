using System;
using System.Collections.Concurrent;
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
                            channel = new Channel
                            {
                                Name = content[1]
                            };

                            _cachedChannels.TryAdd(content[1], channel);
                        }

                        if (!channel._users.Any(x => x == user))
                        {
                            channel._users.Add(new ChannelUser(this, user, channel));
                        }

                        if (!user._channels.Any(x => x == channel))
                        {
                            user._channels.Add(channel);
                        }

                        return;
                    }
                case "PART":
                    {
                        return;
                    }
                case "QUIT":
                    {
                        return;
                    }
                case "NICK":
                    {
                        return;
                    }
                case "PRIVMSG":
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
                            user = new User(this);
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
                        if (_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user.Server = data[3];
                        }

                        return;
                    }

                case 313:
                    {
                        if (_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user.IRCOperator = true;
                        }

                        return;
                    }

                case 317:
                    {
                        if (_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user.Idle = TimeSpan.FromSeconds(int.Parse(data[3]));
                            user.Signon = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data[4]));
                        }

                        return;
                    }

                case 338:
                    {
                        if (_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user.ReverseIp = data[3];
                            user.Ip = data[4];
                        }

                        return;
                    }

                case 332:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel
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
                            channel = new Channel
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
                            channel = new Channel
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

                            user._channels.Add(channel);

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
                                    channelUser.Privileges = ChannelPrivilege.Normal | ChannelPrivilege.Unknown;
                                    break;
                            }

                            channel._users.Add(channelUser);
                        }

                        return;
                    }

                case 324:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        if (data[3].Length > 1)
                        {
                            channel._modes.AddRange(data[3].Substring(1));
                        }

                        return;
                    }

                case 329:
                    {
                        if (!_cachedChannels.TryGetValue(data[2], out var channel))
                        {
                            channel = new Channel
                            {
                                Name = data[2]
                            };

                            _cachedChannels.TryAdd(data[2], channel);
                        }

                        channel.CreatedAt = DateTimeOffset.FromUnixTimeSeconds(int.Parse(data[3]));
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
