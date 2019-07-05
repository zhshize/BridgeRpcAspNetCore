using System;
using BridgeRpc.Abstraction;
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
        
        public event Action<IRpcHub, IServiceProvider> OnConnected;
        public event Action OnDisonnected;
        public event Action<Exception> OnConnectFailed;

        public void Start()
        {
            var services = _scope.ServiceProvider;
            
            var connection = services.GetRequiredService<Connection>();
            connection.OnConnected += (hub, provider) => OnConnected?.Invoke(hub, provider);
            connection.OnDisonnected += () => OnDisonnected?.Invoke();
            connection.OnConnectFailed += e => OnConnectFailed?.Invoke(e);
            connection.Run();
        }
    }
}