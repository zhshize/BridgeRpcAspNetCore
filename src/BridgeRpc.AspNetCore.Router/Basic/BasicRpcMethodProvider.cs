using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRpcMethodProvider : IRpcMethodProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private List<RpcController> _controllers;

        public BasicRpcMethodProvider(IServiceProvider serviceProvider, IRpcControllerProvider controllerProvider)
        {
            _serviceProvider = serviceProvider;
            _controllers = controllerProvider.GetControllers();
        }

        public RoutingPath Path { get; set; }
        public string ClientId { get; set; } = null;

        public List<IRpcMethod> GetAllMethods()
        {
            var methods = new List<IRpcMethod>();

            if (ClientId == null) // this side is seen as a server
            {
                foreach (var controller in _controllers)
                {
                    var attribute = controller.GetType().GetCustomAttribute<RpcRouteAttribute>(true);
                    if (attribute?.RouteName == null || attribute.RouteName == Path)
                    {
                        // Find all public methods and without object methods like GetHashCode, ToString...
                        var all = controller.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => m.DeclaringType != typeof(object));
                        methods.AddRange(
                            all.Select(m => new RpcMethod {Prototype = m, Controller = controller}));
                    }
                }
            }
            else // as client
            {
                foreach (var controller in _controllers)
                {
                    var attribute = controller.GetType().GetCustomAttribute<RpcClientAttribute>(true);
                    if (attribute?.ClientId == null || attribute.ClientId == ClientId)
                    {
                        // Find all public methods and without object methods like GetHashCode, ToString...
                        var all = controller.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                            .Where(m => m.DeclaringType != typeof(object));
                        methods.AddRange(
                            all.Select(m => new RpcMethod() {Prototype = m, Controller = controller}));
                    }
                }
            }


            return methods;
        }
    }
}