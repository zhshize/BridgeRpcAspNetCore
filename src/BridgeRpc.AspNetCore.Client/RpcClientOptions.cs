using System;
using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Client
{
    /// <summary>
    /// RPC connection options for clients
    /// </summary>
    public class RpcClientOptions
    {
        /// <summary>
        /// Create a RpcClientOptions object with default options
        /// </summary>
        public RpcClientOptions()
        {
            RpcOptions = new RpcOptions();
        }
        
        /// <summary>
        /// The RpcOptions object
        /// </summary>
        public RpcOptions RpcOptions { get; set; }
        
        /// <summary>
        /// The URL of server to be connected
        /// </summary>
        public Uri Host { get; set; } = new Uri("ws://localhost");
        
        /// <summary>
        /// Reconnect interval
        /// </summary>
        public TimeSpan? ReconnectInterval { get; set; } = TimeSpan.FromSeconds(60);
        
        /// <summary>
        /// Client id to specify which controllers to handle requests
        /// </summary>
        public string ClientId { get; set; } = "client1";
    }
}