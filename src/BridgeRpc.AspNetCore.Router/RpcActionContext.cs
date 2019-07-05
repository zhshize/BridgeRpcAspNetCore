using BridgeRpc.Abstraction;
using Microsoft.AspNetCore.Http;

namespace BridgeRpc.AspNetCore.Router
{
    public class RpcActionContext
    {
        public IRpcHub Hub { get; protected set; }
        public RpcRequest Request { get; protected set; }
        public RpcResponse Response { get; set; }
        public HttpContext HttpContext { get; protected set; }
    }
}