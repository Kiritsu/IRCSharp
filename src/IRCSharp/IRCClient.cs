using System;
using System.Collections.Concurrent;
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

        private event Action<string> DataReceived;

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
        ///     Configuration of this <see cref="IRCClient"/>.
        /// </summary>
        public IRCConfiguration Configuration { get; }

        /// <summary>
        ///     Creates a new <see cref="IRCClient"/>.
        /// </summary>
        public IRCClient(IRCConfiguration configuration)
        {
            DataReceived += OnDataReceived;

            Configuration = new IRCConfiguration(configuration);

            _tcp = new TcpClient();
            _cachedUsers = new ConcurrentDictionary<string, User>();
            _cachedChannels = new ConcurrentDictionary<string, Channel>();
        }

        /// <summary>
        ///     Connects to the remote server.
        /// </summary>
        public void Connect()
        {
            _tcp.Connect(Configuration.Hostname, Configuration.Port);
            _writer = new StreamWriter(_tcp.GetStream());

            //Sending authentication.
            if (!string.IsNullOrWhiteSpace(Configuration.Password))
            {
                Send($"PASS {Configuration.Password}");
            }

            Send($"NICK {Configuration.Username}");
            Send($"USER {Configuration.Identd} 0 * :{Configuration.RealName}");

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
                    Configuration.Host = sender;
                    HandleServerData(content.Skip(1).ToArray(), data);
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

                    CurrentUser = new User
                    {
                        Username = username,
                    };

                    _cachedUsers.TryAdd(username, CurrentUser);

                    Ready?.Invoke(new ReadyEventArgs
                    {
                        Client = this,
                        CurrentUser = CurrentUser
                    });

                    Send($"WHOIS {Configuration.Host ?? username} {username}");
                    break;

                case 311:
                    {
                        if (!_cachedUsers.TryGetValue(data[2], out var user))
                        {
                            user = new User();
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
                                user = new User { Username = name };
                                _cachedUsers.TryAdd(name, user);
                            }

                            user._channels.Add(channel);

                            var channelUser = new ChannelUser(user);

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
