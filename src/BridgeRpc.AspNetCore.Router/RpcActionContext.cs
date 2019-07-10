using BridgeRpc.Abstraction;
using BridgeRpc.AspNetCore.Router.Abstraction;
using Microsoft.AspNetCore.Http;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    /// Context in a RPC calling.
    /// </summary>
    public class RpcActionContext : IRpcActionContext
    {
        public IRpcHub Hub { get; set; }
        public RpcRequest Request { get; set; }
        public RpcResponse Response { get; set; }

        public void Respond(RpcResponse response)
        {
            Response = response;
        }
    }
}