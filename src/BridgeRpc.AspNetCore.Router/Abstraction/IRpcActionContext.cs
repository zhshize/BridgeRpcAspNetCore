using BridgeRpc.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcActionContext
    {
        IRpcHub Hub { get; }
        RpcRequest Request { get; }
        RpcResponse Response { get; set; }
    }
}