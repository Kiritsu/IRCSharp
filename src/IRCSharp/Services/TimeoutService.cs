using System;
using System.Threading;

namespace IRCSharp.Services
{
    internal sealed class TimeoutService
    {
        private readonly IRCClient _client;

        private DateTimeOffset LastPing { get; set; }

        public TimeoutService(IRCClient client)
        {
            _client = client;

            LastPing = DateTimeOffset.Now;
        }

        public void Run(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (LastPing < DateTimeOffset.Now - TimeSpan.FromMinutes(5))
                {
                    _client.Disconnect(_client._configuration.ThrowsOnTimeout);
                    return;
                }

                Thread.Sleep(5000);
            }
        }

        public void UpdatePing()
        {
            LastPing = DateTimeOffset.Now;
        }
    }
}
