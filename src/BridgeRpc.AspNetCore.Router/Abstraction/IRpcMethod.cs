using System.Reflection;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcMethod
    {
        MethodInfo Prototype { get; }
        RpcController Controller { get; }
    }
}