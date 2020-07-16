using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRCSharp
{
    public class IRCClient : IDisposable
    {
        private const int MessageBufferSize = 0x200;

        private readonly Memory<byte> _sendBuffer;
        private readonly Memory<byte> _receiveBuffer;

        private readonly IRCConfiguration _configuration;
        private readonly Socket _socket;

        private readonly Memory<byte> _incompletePacketBuffer;

        /// <summary>
        ///     Creates a new IRCClient.
        /// </summary>
        /// <param name="configuration">Configuration of the client.</param>
        /// <exception cref="ArgumentNullException">Thrown when the configuration is malformed.</exception>
        public IRCClient(IRCConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration), "Configuration must not be null.");

            if (configuration.Hostname is null)
                throw new ArgumentNullException(nameof(configuration.Hostname), "Hostname must not be null.");

            configuration.Nickname ??= "user";

            _configuration = configuration;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _sendBuffer = new byte[MessageBufferSize];
            _receiveBuffer = new byte[MessageBufferSize];
            _incompletePacketBuffer = new byte[MessageBufferSize];
        }

        /// <summary>
        ///     Asynchronously connects and authenticates to the remote server.
        /// </summary>
        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync(_configuration.Hostname, _configuration.Port).ConfigureAwait(false);
            await SendAuthenticationAsync().ConfigureAwait(false);

            _ = Task.Run(async () =>
            {
                while (_socket.Connected)
                {
                    _receiveBuffer.Span.Clear();
                    await _socket.ReceiveAsync(_receiveBuffer, SocketFlags.None).ConfigureAwait(false);

                    var offset = 0;
                    do
                    {
                        var packetEnd = _receiveBuffer.Span.Slice(offset)
                            .IndexOf(PacketHelpers.LineEnding.Span);

                        if (packetEnd == -1)
                        {
                            // no complete packet, copy incomplete one for future concat
                            _receiveBuffer.Span.Slice(offset).CopyTo(_incompletePacketBuffer.Span[..]);

                            break;
                        }

                        var packet = MemoryHelpers.Concat<byte>(
                            // cannot fail
                            _incompletePacketBuffer.Slice(0,
                                _incompletePacketBuffer.Span.IndexOf(PacketHelpers.ByteZero.Span)),
                            _receiveBuffer.Slice(offset, packetEnd));

                        offset += packetEnd + 1;

                        await HandlePacketAsync(packet).ConfigureAwait(false);
                        
                        // clear handled incomplete packet.
                        _incompletePacketBuffer.Span.Clear();
                    } while (true);
                }
            });
        }

        public async Task SetNicknameAsync(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                throw new ArgumentException("Nickname cannot be null or empty.", nameof(nickname));
            }

            await SendBytesAsync(PacketHelpers.NicknamePacketHeader).ConfigureAwait(false);
            await SendAsync(nickname.AsMemory()).ConfigureAwait(false);
            await SendEndOfLineAsync().ConfigureAwait(false);
        }

        private Task HandlePacketAsync(ReadOnlyMemory<byte> packet)
        {
            var str = Encoding.UTF8.GetString(packet.Span);
            Console.WriteLine(str);

            return Task.CompletedTask;
        }
        
        private async Task SendAuthenticationAsync()
        {
            if (!string.IsNullOrWhiteSpace(_configuration.Password))
            {
                await SendBytesAsync(PacketHelpers.PassPacketHeader).ConfigureAwait(false);
                await SendAsync(_configuration.Password.AsMemory()).ConfigureAwait(false);
                await SendEndOfLineAsync().ConfigureAwait(false);
            }

            await SetNicknameAsync(_configuration.Nickname).ConfigureAwait(false);
            await SendAsync($"USER {_configuration.Identd} 0 * :{_configuration.Realname}".AsMemory())
                .ConfigureAwait(false);
        }

        private ValueTask<int> SendEndOfLineAsync()
        {
            return SendBytesAsync(PacketHelpers.LineEnding);
        }

        private ValueTask<int> SendAsync(ReadOnlyMemory<char> message)
        {
            Encoding.UTF8.GetBytes(message.Span, _sendBuffer.Span);
            return SendBytesAsync(_sendBuffer);
        }

        private ValueTask<int> SendBytesAsync(ReadOnlyMemory<byte> message)
        {
            return _socket.SendAsync(message, SocketFlags.None);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}