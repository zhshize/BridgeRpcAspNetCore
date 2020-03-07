using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Basic;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using Timer = System.Timers.Timer;

namespace BridgeRpc.AspNetCore.Client
{
    public class Connection
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public Connection(IServiceScopeFactory scopeFactory, RpcClientOptions options)
        {
            _scopeFactory = scopeFactory;
            Options = options;
        }

        public RpcClientOptions Options { get; set; }
        public bool Reconnect { get; set; } = false;

        public event Action<IRpcHub, IServiceProvider> OnConnected;
        public event Action OnDisconnected;
        public event Action<Exception> OnConnectFailed;
        private bool _needDisconnect = false;

        public void Run()
        {
            Task.Run(async () =>
            {
                while (_needDisconnect == false)
                {
                    var client = ReconnectSocket();
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
                                var hub = scope.ServiceProvider.GetService<IRpcHub>();
                                hub.OnReservedRequest += request =>
                                    request.Method == ".ping" ? new RpcResponse() : null;
                                var pingTimer = new Timer
                                {
                                    Interval = Options.PingTimeout.TotalMilliseconds,
                                    AutoReset = false
                                };
                                pingTimer.Elapsed += (sender, args) => hub.Disconnect();
                                pingTimer.Enabled = true;
#pragma warning disable 4014
                                Task.Run(() =>
#pragma warning restore 4014
                                {
                                    // ReSharper disable once AccessToDisposedClosure
                                    OnConnected?.Invoke(hub, scope.ServiceProvider);
                                });
                                await socket.Start();
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(e);
                            }
                        }

                        OnDisconnected?.Invoke();
                    }

                    if (Reconnect)
                    {
                        if (Options.ReconnectInterval.HasValue)
                            await Task.Delay(Options.ReconnectInterval.Value);
                    }
                    else
                    {
                        _needDisconnect = true;
                    }
                }
            });
        }

        private ClientWebSocket ReconnectSocket()
        {
            var client = new ClientWebSocket();
            client.Options.KeepAliveInterval = Options.RpcOptions.KeepAliveInterval;
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