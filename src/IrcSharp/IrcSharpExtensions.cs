namespace IrcSharp;

public static class IrcSharpExtensions
{
    extension(IrcClient client)
    {
        public Task NickAsync(string nick, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"NICK {nick}", cancellationToken);
        
        public Task JoinAsync(string channel, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"JOIN {channel}", cancellationToken);
        
        public Task UserAsync(string identd, string realname, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"USER {identd} 0 * :{realname}", cancellationToken);
        
        public Task PassAsync(string password, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"PASS {password}", cancellationToken);
        
        public Task PongAsync(string? token, CancellationToken cancellationToken = default) 
            => client.SendRawMessageAsync($"PONG :{token}", cancellationToken);
    }
}