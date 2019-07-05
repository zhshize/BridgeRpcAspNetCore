using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Client;
using MessagePack;
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
                var client = new RpcIndependentScopeClient(serviceScope);

                try
                {
                    client.OnConnected += async (hub, provider) =>
                    {
                        while (true)
                        {
                            var r = await hub.RequestAsync("greet", MessagePackSerializer.Serialize("Joe"));
                            Console.WriteLine(r.GetResult<string>());
                            await Task.Delay(5000);
                        }
                    };
                    client.OnConnectFailed += e => Console.WriteLine("Cannot connect to the server");
                    client.OnDisonnected += () => Console.WriteLine("Disconnected");
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}