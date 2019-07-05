using System;
using System.Collections.Generic;
using System.Linq;
using BridgeRpc.AspNetCore.Router.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRpcControllerProvider : IRpcControllerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public BasicRpcControllerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<RpcController> GetControllers()
        {
            return _serviceProvider.GetServices<RpcController>().ToList();
        }
    }
}