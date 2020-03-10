using System;
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


        /// <summary>
        ///     Ping interval (default 10 seconds)
        /// </summary>
        public TimeSpan PingInterval { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        ///     Pong timeout (default 3 seconds)
        /// </summary>
        public TimeSpan PongTimeout { get; set; } = TimeSpan.FromSeconds(3);

        public RoutingOptions RoutingOptions { get; set; }
        public RpcOptions RpcOptions { get; set; }
    }
}