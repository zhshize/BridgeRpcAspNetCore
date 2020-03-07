using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    ///     Context in a RPC calling.
    /// </summary>
    public class RpcActionContext : IRpcActionContext
    {
        /// <summary>
        ///     The <see cref="IRpcHub" /> that handling this request
        /// </summary>
        public IRpcHub Hub { get; set; }

        /// <summary>
        ///     The request object
        /// </summary>
        public RpcRequest Request { get; set; }

        /// <summary>
        ///     The response will be sent
        ///     Keep this object from replacing is good idea to increase performance
        /// </summary>
        public RpcResponse Response { get; set; }

        /// <summary>
        ///     Force to replace the response object
        /// </summary>
        /// <param name="response"></param>
        public void Respond(RpcResponse response)
        {
            Response = response;
        }
    }
}