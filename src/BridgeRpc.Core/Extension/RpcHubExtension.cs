using System;
using System.Threading.Tasks;
using BridgeRpc.Core.Abstraction;
using MessagePack;

namespace BridgeRpc.Core.Extension
{
    public static class RpcHubExtension
    {
        public static Task<RpcResponse> RequestAsync<T>(this IRpcHub hub, string method, T data)
        {
            return hub.RequestAsync(method, MessagePackSerializer.Serialize(data));
        }
        
        public static Task<RpcResponse> RequestAsync<T>(this IRpcHub hub, string method, T data, TimeSpan timeout)
        {
            return hub.RequestAsync(method, MessagePackSerializer.Serialize(data), timeout);
        }
        
        public static void Notify<T>(this IRpcHub hub, string method, T data)
        {
            hub.Notify(method, MessagePackSerializer.Serialize(data));
        }
    }
}