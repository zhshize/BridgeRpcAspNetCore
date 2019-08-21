using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IMethodInvoker
    {
        /// <summary>
        /// Try matching parameter and invoke method by request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        RpcResponse Call(IRpcMethod method, ref IRpcActionContext context);

        /// <summary>
        /// Try matching parameter and invoke method by notification request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="context"></param>
        void Notify(IRpcMethod method, ref IRpcActionContext context);
    }
}