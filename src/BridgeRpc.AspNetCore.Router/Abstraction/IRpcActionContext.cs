using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    /// <summary>
    ///     Context in a RPC calling.
    /// </summary>
    public interface IRpcActionContext
    {
        /// <summary>
        ///     The <see cref="IRpcHub" /> that handling this request
        /// </summary>
        IRpcHub Hub { get; set; }

        /// <summary>
        ///     The request object
        /// </summary>
        RpcRequest Request { get; set; }

        /// <summary>
        ///     The response will be sent
        ///     Keep this object from replacing is good idea to increase performance
        /// </summary>
        RpcResponse Response { get; set; }
    }
}