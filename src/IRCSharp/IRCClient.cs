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
        private static readonly Memory<byte> SendBuffer;
        private static readonly Memory<byte> ReceiveBuffer;
        
        private readonly IRCConfiguration _configuration;
        private readonly Socket _socket;

        static IRCClient()
        {
            SendBuffer = new Memory<byte>(new byte[MessageBufferSize]);
            ReceiveBuffer = new Memory<byte>(new byte[MessageBufferSize]);
            EndOfLineBuffer = new[] {(byte) '\n'};
            NicknamePacketBuffer = new[] {(byte) 'N', (byte) 'I', (byte) 'C', (byte) 'K', (byte) ' '};
            PassPacketBuffer = new[] {(byte) 'P', (byte) 'A', (byte) 'S', (byte) 'S', (byte) ' '};
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
        }

        public async Task ConnectAsync()
        {
            await _socket.ConnectAsync(_configuration.Hostname, _configuration.Port).ConfigureAwait(false);
            await SendAuthenticationAsync().ConfigureAwait(false);

            _ = Task.Run(async () =>
            {
                while (_socket.Connected)
                {
                    try
                    {
                        ReceiveBuffer.Span.Clear();
                        await _socket.ReceiveAsync(ReceiveBuffer, SocketFlags.None).ConfigureAwait(false);
                        var content = Encoding.UTF8.GetString(ReceiveBuffer.Span);
                        Console.Write(content);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
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
            Encoding.UTF8.GetBytes(message.Span, SendBuffer.Span);
            await SendBytesAsync(SendBuffer).ConfigureAwait(false);
        }

        private async Task SendBytesAsync(ReadOnlyMemory<byte> message)
        {
            await _socket.SendAsync(message, SocketFlags.None).ConfigureAwait(false);
        }

        public void Dispose()
        {
            _socket.Dispose();
        }
    }
}