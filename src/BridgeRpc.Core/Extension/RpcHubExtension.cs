using System;
using System.Threading.Tasks;
using BridgeRpc.Core.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core.Extension
{
    public static class RpcHubExtension
    {
        /// <summary>
        /// Call other side.
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="method">Method name</param>
        /// <param name="data">Data will be sent</param>
        /// <typeparam name="T">Type of data</typeparam>
        /// <returns><see cref="RpcResponse"/> received from other side</returns>
        public static Task<RpcResponse> RequestAsync<T>(this IRpcHub hub, string method, T data)
        {
            return hub.RequestAsync(method, data);
        }

        /// <summary>
        /// Notify other side.  No response will be sent back.
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="method">Method name</param>
        /// <param name="data">Data will be sent</param>
        /// <typeparam name="T">Type of data</typeparam>
        public static void Notify<T>(this IRpcHub hub, string method, T data)
        {
            hub.Notify(method, data);
        }
    }
}