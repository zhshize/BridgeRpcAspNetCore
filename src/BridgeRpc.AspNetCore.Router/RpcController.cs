using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    ///     Base class for RpcControllers
    /// </summary>
    public abstract class RpcController
    {
        /// <summary>
        ///     Request calling action context.
        ///     This property is automatically injected.  DO NOT modify this.
        /// </summary>
        public IRpcActionContext RpcContext { get; set; }
        // TODO: Inject context of Rpc Calling
        // TODO: Filter (middleware) before and after method call
    }
}