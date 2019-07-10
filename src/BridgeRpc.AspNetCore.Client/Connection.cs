using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BridgeRpc.Abstraction;
using BridgeRpc.AspNetCore.Router.Basic;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Client
{
    public class Connection
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public RpcClientOptions Options { get; set; }

        public Connection(IServiceScopeFactory scopeFactory, RpcClientOptions options)
        {
            _scopeFactory = scopeFactory;
            Options = options;
        }

        public event Action<IRpcHub, IServiceProvider> OnConnected;
        public event Action OnDisonnected;
        public event Action<Exception> OnConnectFailed;

        public void Run()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var client = Reconnect();
                    if (client != null)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var socket = new BasicSocket(Options.RpcOptions);
                            socket.SetSocket(client);
                            scope.ServiceProvider.GetService<SocketProvider>().Socket = socket;
                            try
                            {
                                var router = scope.ServiceProvider.GetService<BasicRouter>();
                                router.ClientId = Options.ClientId;
                                var handler = scope.ServiceProvider.GetService<IRpcHub>();
                                // TODO: socket initialize
#pragma warning disable 4014
                                Task.Run(() =>
#pragma warning restore 4014
                                {
                                    if (scope != null) OnConnected?.Invoke(handler, scope.ServiceProvider);
                                });
                                await socket.Start();
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e);
                            }
                        }

                        OnDisonnected?.Invoke();
                    }

                    if (Options.ReconnectInterval.HasValue)
                        await Task.Delay(Options.ReconnectInterval.Value);
                }
            });
        }

        private ClientWebSocket Reconnect()
        {
            var client = new ClientWebSocket();
            try
            {
                client.ConnectAsync(Options.Host, CancellationToken.None).Wait();
            }
            catch (Exception e)
            {
                OnConnectFailed?.Invoke(e);
                return null;
            }

            return client;
        }
    }
}