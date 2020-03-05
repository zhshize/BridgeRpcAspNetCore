using System;
using System.Threading.Tasks;
using BridgeRpc.Core.Abstraction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core.Extension
{
    public static class RpcHubExtension
    {
        public static Task<RpcResponse> RequestAsync<T>(this IRpcHub hub, string method, T data)
        {
            return hub.RequestAsync(method, data);
        }

        public static void Notify<T>(this IRpcHub hub, string method, T data)
        {
            hub.Notify(method, data);
        }
    }
}