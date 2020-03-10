using System;
using BridgeRpc.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Client
{
    public class RpcIndependentScopeClient
    {
        private readonly IServiceScope _scope;

        public RpcIndependentScopeClient(IServiceScope scope)
        {
            _scope = scope;
        }

        public RpcIndependentScopeClient(IServiceScope scope, RpcClientOptions options)
        {
            _scope = scope;
            Options = options;
        }

        public RpcClientOptions Options { get; set; }

        public event Action<IRpcHub, IServiceProvider> OnConnected;
        public event Action OnDisconnected;
        public event Action<Exception> OnConnectFailed;

        public void Start()
        {
            var services = _scope.ServiceProvider;

            var connection = services.GetRequiredService<Connection>();
            if (Options != null) connection.Options = Options;
            connection.OnConnected += (hub, provider) => OnConnected?.Invoke(hub, provider);
            connection.OnDisconnected += () => OnDisconnected?.Invoke();
            connection.OnConnectFailed += e => OnConnectFailed?.Invoke(e);
            connection.Run();
        }
    }
}