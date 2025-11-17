// See https://aka.ms/new-console-template for more information

using IrcSharp;
using IrcSharp.Playground;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = Host.CreateApplicationBuilder(args);

host.Services.AddSingleton<IrcClient>(x => new IrcClient(
    x.GetRequiredService<ILogger<IrcClient>>(), 
    Options.Create(new IrcOptions
    {
        Host = "irc.freenode.net",
        Port = 6667,
        UseSsl = false,
        UseSslWithNoValidation = false,
        Username = "testuser19567",
        IgnoreUnknownMessages = false,
        WaitForHandlersBeforeNextMessage = false
    })));

host.Services.AddHostedService<IrcService>();

host.Build().Run();