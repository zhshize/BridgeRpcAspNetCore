using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Server
{
    public class RpcServerOptions
    {
        public RpcServerOptions()
        {
            RoutingOptions = new RoutingOptions();
            RpcOptions = new RpcOptions();
        }
        
        public RoutingOptions RoutingOptions { get; set; }
        public RpcOptions RpcOptions { get; set; }
    }
}