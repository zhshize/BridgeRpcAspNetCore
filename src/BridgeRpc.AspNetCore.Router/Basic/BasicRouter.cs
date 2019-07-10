using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BridgeRpc.Abstraction;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Pipeline;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRouter
    {
        public BasicRouter(IServiceProvider serviceProvider, IRpcHub hub, IRpcMethodProvider methodProvider,
            IHttpContextAccessor httpContextAccessor, IPipeline pipeline)
        {
            _serviceProvider = serviceProvider;
            _hub = hub;
            _methodProvider = methodProvider;
            _pipeline = pipeline;

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
        private readonly IPipeline _pipeline;
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
            IRpcActionContext context = new RpcActionContext
            {
                Hub = _hub,
                Request = request,
                Response = new RpcResponse
                {
                    Id = request.Id
                }
            };

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
                    _pipeline.ProcessRequestAsync(method, context).Wait();
                    //_methodInvoker.Notify(method, ref context);
                }

                return null;
            }
            else
            {
                var method = methods.FirstOrDefault();
                if (method == null) return context.Response;
                
                _pipeline.ProcessRequestAsync(method, context).Wait();
                //var res = _methodInvoker.Call(method, ref context);

                return context.Response;
            }
        }

        private string GetMethodName(MethodInfo m)
        {
            var attribute = m.GetCustomAttributes<RpcMethodAttribute>().FirstOrDefault();
            return attribute == null ? m.Name : attribute.MethodName;
        }
    }
}