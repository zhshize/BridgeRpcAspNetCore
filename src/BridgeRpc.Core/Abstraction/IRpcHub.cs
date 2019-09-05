using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeRpc.Core.Abstraction
{
    public delegate RpcResponse RequestHandler(RpcRequest request);
    public delegate void DisconnectHandler();
    
    /// <summary>
    /// Handling and requesting with other side.
    /// </summary>
    public interface IRpcHub
    {
        Dictionary<string, object> Items { get; }
        /// <summary>
        /// Invoke with <see cref="RpcRequest"/> object when other side requests.
        /// </summary>
        event RequestHandler OnRequest;
        
        /// <summary>
        /// Invoke when disconnected.
        /// </summary>
        event DisconnectHandler OnDisconnect;

        /// <summary>
        /// Call other side.
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="data">Binary data will be sent</param>
        /// <returns><see cref="RpcResponse"/> sent from other side</returns>
        Task<RpcResponse> RequestAsync(string method, byte[] data);

        /// <summary>
        /// Call other side.
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="data">Binary data will be sent</param>
        /// <param name="timeout">Request timeout</param>
        /// <returns><see cref="RpcResponse"/> sent from other side</returns>
        Task<RpcResponse> RequestAsync(string method, byte[] data, TimeSpan timeout);

        /// <summary>
        /// Notify other side.  No response will be sent back.
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="data">Binary data will be sent</param>
        void Notify(string method, byte[] data);

        /// <summary>
        /// Disconnect.
        /// </summary>
        void Disconnect();
    }
}