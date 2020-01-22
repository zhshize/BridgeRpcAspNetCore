using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BridgeRpc.Core.Abstraction
{
    public delegate RpcResponse RequestHandler(RpcRequest request);
    public delegate void DisconnectHandler();
    public delegate void OnMessageExceptionHandler(Exception e, byte[] message);
    public delegate void OnRequestInvokingExceptionHandler(Exception e, byte[] message);
    
    /// <summary>
    /// Handling and requesting with other side.
    /// </summary>
    public interface IRpcHub
    {
        /// <summary>
        /// Custom data for a connection.
        /// </summary>
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
        /// Invoke when an Exception object thrown during the data received
        /// </summary>
        event OnMessageExceptionHandler OnMessageException;
        
        /// <summary>
        /// 
        /// </summary>
        event OnRequestInvokingExceptionHandler OnRequestInvokingException;

        /// <summary>
        /// Call other side.
        /// </summary>
        /// <param name="method">Method name</param>
        /// <param name="data">Binary data will be sent</param>
        /// <param name="throwRpcException">If true, when the Error field preset in the response,
        /// this task will throw the error as a <see cref="RpcException"/>.</param>
        /// <param name="timeout">Request timeout</param>
        /// <returns><see cref="RpcResponse"/> sent from other side</returns>
        Task<RpcResponse> RequestAsync(string method, byte[] data, bool throwRpcException = false, TimeSpan? timeout = null);

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