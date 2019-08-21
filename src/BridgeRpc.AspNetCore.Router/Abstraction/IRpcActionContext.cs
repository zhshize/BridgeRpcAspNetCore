using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcActionContext
    {
        IRpcHub Hub { get; }
        RpcRequest Request { get; }
        RpcResponse Response { get; set; }
    }
}