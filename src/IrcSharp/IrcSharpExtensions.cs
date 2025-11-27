namespace IrcSharp;

public static class IrcSharpExtensions
{
    extension(IrcClient client)
    {
        public Task CapLsAsync(string version, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"CAP LS {version}", cancellationToken);
        
        public Task CapReqAsync(string capability, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"CAP REQ :{capability}", cancellationToken);
        
        public Task NickAsync(string nick, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"NICK {nick}", cancellationToken);
        
        public Task UserAsync(string identd, string realname, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"USER {identd} 0 * :{realname}", cancellationToken);
        
        public Task PassAsync(string password, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"PASS {password}", cancellationToken);
        
        public Task PongAsync(string? token, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"PONG :{token}", cancellationToken);
        
        public Task OperAsync(string user, string password, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"OPER {user} {password}", cancellationToken);

        public Task QuitAsync(CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync("QUIT".AsMemory(), cancellationToken);
        
        public Task QuitAsync(string? message, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"QUIT :{message}", cancellationToken);

        public Task JoinAsync(string channel, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"JOIN {channel}", cancellationToken);
        
        public Task JoinAsync(string channel, string key, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"JOIN {channel} {key}", cancellationToken);
        
        public Task JoinAsync(IEnumerable<string> channels, IEnumerable<string> keys, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"JOIN {string.Join(",", channels)} {string.Join(",", keys)}", cancellationToken);
        
        public Task PartAsync(string channel, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"PART {channel}", cancellationToken);
        
        public Task PartAsync(IEnumerable<string> channels, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"PART {string.Join(",", channels)}", cancellationToken);
        
        public Task PartAsync(string channel, string message, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"PART {channel} :{message}", cancellationToken);
        
        public Task PartAsync(IEnumerable<string> channels, string message, CancellationToken cancellationToken = default)
            => client.SendRawMessageAsync($"PART {string.Join(",", channels)} :{message}", cancellationToken);
    }
}