using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcActionContext
    {
        IRpcHub Hub { get; set; }
        RpcRequest Request { get; set; }
        RpcResponse Response { get; set; }
    }
}