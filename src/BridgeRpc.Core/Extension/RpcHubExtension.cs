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
            if (typeof(T) == typeof(string))
                return hub.RequestAsync(method, new JRaw("\"" + data + "\""));

            return hub.RequestAsync(method, new JRaw(data));
        }
        
        public static Task<RpcResponse> RequestAsync<T>(this IRpcHub hub, string method, T data, TimeSpan timeout)
        {
            if (typeof(T) == typeof(string))
                return hub.RequestAsync(method, new JRaw("\"" + data + "\""), timeout);
            
            return hub.RequestAsync(method, new JRaw(data), timeout);
        }
        
        public static void Notify<T>(this IRpcHub hub, string method, T data)
        {
            if (typeof(T) == typeof(string))
                hub.Notify(method, new JRaw("\"" + data + "\""));
            
            hub.Notify(method, new JRaw(data));
        }
    }
}