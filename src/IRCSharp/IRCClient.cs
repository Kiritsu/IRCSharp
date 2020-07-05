using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IRCSharp
{
    public class IRCClient : IDisposable
    {
        private const int MessageBufferSize = 0x200;

        private static readonly Memory<byte> NicknamePacketBuffer;
        private static readonly Memory<byte> PassPacketBuffer;
        private static readonly Memory<byte> EndOfLineBuffer;

        private static readonly Memory<byte> IncompletePacketBuffer;

        private readonly Memory<byte> _sendBuffer;
        private readonly Memory<byte> _receiveBuffer;

        private readonly IRCConfiguration _configuration;
        private readonly Socket _socket;

        static IRCClient()
        {
            EndOfLineBuffer = new[] {(byte) '\n'};
            NicknamePacketBuffer = new[] {(byte) 'N', (byte) 'I', (byte) 'C', (byte) 'K', (byte) ' '};
            PassPacketBuffer = new[] {(byte) 'P', (byte) 'A', (byte) 'S', (byte) 'S', (byte) ' '};
            IncompletePacketBuffer = new Memory<byte>(new byte[MessageBufferSize]);
        }

        public IRCClient(IRCConfiguration configuration)
        {
            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration), "Configuration must not be null.");

            if (configuration.Hostname is null)
                throw new ArgumentNullException(nameof(configuration.Hostname), "Hostname must not be null.");

            configuration.Nickname ??= "user";

            _configuration = configuration;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            _sendBuffer = new Memory<byte>(new byte[MessageBufferSize]);
            _receiveBuffer = new Memory<byte>(new byte[MessageBufferSize]);
        }

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

                    var index = 0;
                    int packetEndIndex;
                    var subBuffer = new Memory<byte>(new byte[_receiveBuffer.Length]);
                    _receiveBuffer.CopyTo(subBuffer);
                    do
                    {
                        packetEndIndex = subBuffer.Span.IndexOf(EndOfLineBuffer.Span);
                        subBuffer = _receiveBuffer.Slice(index, packetEndIndex);
                        index += packetEndIndex;
                        
                        if (!IncompletePacketBuffer.IsEmpty && IncompletePacketBuffer.Length > 0)
                        {
                            var memory = new Memory<byte>(new byte[subBuffer.Length + IncompletePacketBuffer.Length]);
                            IncompletePacketBuffer.CopyTo(memory);
                            for (var i = 0; i < subBuffer.Span.Length; i++)
                            {
                                memory.Span.Fill(subBuffer.Span[i]);
                            }

                            await HandlePacketAsync(memory);
                        }
                        else
                        {
                            await HandlePacketAsync(subBuffer);   
                        }
                    } while (packetEndIndex != -1);

                    if (index < _receiveBuffer.Length)
                    {
                        var unfinishedPacket = _receiveBuffer.Slice(index, _receiveBuffer.Length - index);
                        unfinishedPacket.CopyTo(IncompletePacketBuffer);
                    }
                }
            });
        }

        private async Task HandlePacketAsync(ReadOnlyMemory<byte> packet)
        {
        }

        public async Task SetNicknameAsync(string nickname)
        {
            if (string.IsNullOrWhiteSpace(nickname))
            {
                throw new ArgumentException("Nickname cannot be null or empty.", nameof(nickname));
            }

            await SendBytesAsync(NicknamePacketBuffer).ConfigureAwait(false);
            await SendAsync(nickname.AsMemory()).ConfigureAwait(false);
            await SendEndOfLineAsync().ConfigureAwait(false);
        }

        private async Task SendAuthenticationAsync()
        {
            if (!string.IsNullOrWhiteSpace(_configuration.Password))
            {
                await SendBytesAsync(PassPacketBuffer).ConfigureAwait(false);
                await SendAsync(_configuration.Password.AsMemory()).ConfigureAwait(false);
                await SendEndOfLineAsync().ConfigureAwait(false);
            }

            await SetNicknameAsync(_configuration.Nickname).ConfigureAwait(false);
            await SendAsync($"USER {_configuration.Identd} 0 * :{_configuration.Realname}".AsMemory())
                .ConfigureAwait(false);
        }

        private async Task SendEndOfLineAsync()
        {
            await SendBytesAsync(EndOfLineBuffer).ConfigureAwait(false);
        }

        private async Task SendAsync(ReadOnlyMemory<char> message)
        {
            Encoding.UTF8.GetBytes(message.Span, _sendBuffer.Span);
            await SendBytesAsync(_sendBuffer).ConfigureAwait(false);
        }

        private ValueTask<int> SendBytesAsync(ReadOnlyMemory<byte> message)
        {
            return _socket.SendAsync(message, SocketFlags.None);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}