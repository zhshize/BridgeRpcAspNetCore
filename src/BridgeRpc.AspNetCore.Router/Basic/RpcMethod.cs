using System.Reflection;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class RpcMethod : IRpcMethod
    {
        public MethodInfo Prototype { get; set; }
        public RpcController Controller { get; set; }
    }
}