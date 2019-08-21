using System;
using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Client
{
    public class RpcClientOptions
    {
        public RpcClientOptions()
        {
            RpcOptions = new RpcOptions();
        }
        public RpcOptions RpcOptions { get; set; }
        public Uri Host { get; set; } = new Uri("ws://localhost");
        public TimeSpan? ReconnectInterval { get; set; } = TimeSpan.FromSeconds(60);
        public string ClientId { get; set; } = "client1";
    }
}