using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router
{
    public abstract class RpcController
    {
        public IRpcActionContext RpcContext { get; set; }
        // TODO: Inject context of Rpc Calling
        // TODO: Filter (middleware) before and after method call
    }
}