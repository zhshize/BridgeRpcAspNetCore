﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Client;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                var options = new RpcClientOptions();
                options.Host = new Uri("ws://localhost:5000/go");
                options.ClientId = "client1";
                options.Reconnect = true;
                options.ReconnectInterval = TimeSpan.FromSeconds(6);
                options.PingTimeout = TimeSpan.FromSeconds(60);

                options.RpcOptions.AllowedOrigins = new List<string> {"*"};
                options.RpcOptions.BufferSize = 16 * 1024;
                options.RpcOptions.KeepAliveInterval = TimeSpan.FromSeconds(4);
                options.RpcOptions.RequestTimeout = TimeSpan.FromSeconds(4);

                var client = new RpcIndependentScopeClient(serviceScope, options);

                try
                {
                    client.OnConnected += async (hub, provider) =>
                    {
                        hub.OnMessageException += (exception, message) =>
                        {
                            Console.WriteLine(message);
                            Console.WriteLine(exception);
                        };
                        hub.OnRequestInvokingException += (exception, message) =>
                        {
                            Console.WriteLine(message);
                            Console.WriteLine(exception);
                        };
                        try
                        {
                            while (true)
                            {
                                var r = await hub.RequestAsync("greet", new Dto {Name = "joe"});
                                Console.WriteLine(r.GetResult<string>());
                                await Task.Delay(5000);
                            }
                        }
                        catch
                        {
                            // ignore
                        }
                    };
                    client.OnConnectFailed += e =>
                    {
                        Console.WriteLine("Cannot connect to the server");
                        Console.WriteLine(e);
                    };
                    client.OnDisconnected += () => Console.WriteLine("Disconnected");
                    client.Start();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred.");
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }
    }
}