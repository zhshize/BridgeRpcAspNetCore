using System.Collections.Generic;
using System.Reflection;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRpcControllerPublicMethodProvider : IRpcControllerPublicMethodProvider
    {
        private readonly RoutingPath _path;

        public BasicRpcControllerPublicMethodProvider(RoutingPath path)
        {
            _path = path;
        }
        
        public List<MethodInfo> GetAllMethods()
        {
            throw new System.NotImplementedException();
        }
    }
}