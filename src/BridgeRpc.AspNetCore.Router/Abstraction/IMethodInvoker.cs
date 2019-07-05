namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IMethodInvoker
    {
        /// <summary>
        /// Try matching parameter and invoke method by request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        RpcResponse Call(IRpcMethod method, RpcRequest request);

        /// <summary>
        /// Try matching parameter and invoke method by notification request.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="request">Notification request.</param>
        void Notify(IRpcMethod method, RpcRequest request);
    }
}