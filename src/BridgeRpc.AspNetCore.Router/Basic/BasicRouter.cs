using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BridgeRpc.Abstraction;
using BridgeRpc.AspNetCore.Router.Abstraction;
using Microsoft.AspNetCore.Http;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRouter
    {
        public BasicRouter(IServiceProvider serviceProvider, IRpcHub hub, IRpcMethodProvider methodProvider,
            IMethodInvoker methodInvoker, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _hub = hub;
            _methodProvider = methodProvider;
            _methodInvoker = methodInvoker;

            try
            {
                _methodProvider.Path = RoutingPath.Parse(httpContextAccessor.HttpContext.Request.Path);
            }
            catch (Exception)
            {
                // ignored
            }

            _hub.OnRequest += Handle;
            if (_methodProvider == null)
                throw new NullReferenceException("Argument 'methodProvider' in BasicRouter is null.");
            _methods = _methodProvider.GetAllMethods();
            if (_methods == null)
                throw new NullReferenceException("methodProvider.GetAllMethods() in BasicRouter returned null.");
        }

        private readonly IServiceProvider _serviceProvider;
        private readonly IRpcHub _hub;
        private readonly IRpcMethodProvider _methodProvider;
        private readonly IMethodInvoker _methodInvoker;
        private readonly List<IRpcMethod> _methods;
        private string _clientId;

        /// <summary>
        /// If preset, this side will be seen a client, if null, this side will be seen as a server.
        /// </summary>
        public string ClientId
        {
            get => _clientId;
            set
            {
                _clientId = value;
                _methodProvider.ClientId = value;
            }
        }

        /// <summary>
        /// If request is notification, all matched method will be invoked, if not, the first matched method will 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RpcResponse Handle(RpcRequest request)
        {
            var methodName = request.Method;
            var methods = _methods.Where(m => GetMethodName(m.Prototype) == methodName);

            // insensitive case
            if (_methods.Count == 0)
                methods = _methods
                    .Where(m => string.Equals(GetMethodName(m.Prototype), methodName,
                        StringComparison.CurrentCultureIgnoreCase));
            if (_methods.Count == 0)
            {
                var e = new Exception("Method not found.");
                throw new RpcException(RpcErrorCode.MethodNotFound, e.Message, e);
            }

            if (request.IsNotify())
            {
                foreach (var method in methods)
                {
                    _methodInvoker.Notify(method, request);
                }

                return null;
            }
            else
            {
                RpcResponse res = null;

                foreach (var method in methods)
                {
                    res = _methodInvoker.Call(method, request);
                    if (res != null) break;
                }

                if (res == null)
                    return new RpcResponse
                    {
                        Id = request.Id
                    };


                return res;
            }
        }

        private string GetMethodName(MethodInfo m)
        {
            var attribute = m.GetCustomAttributes<RpcMethodAttribute>().FirstOrDefault();
            return attribute == null ? m.Name : attribute.MethodName;
        }
    }
}