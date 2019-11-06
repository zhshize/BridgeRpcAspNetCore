using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Pipeline;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicRouter
    {
        public BasicRouter(IServiceProvider serviceProvider, IRpcHub hub, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _hub = hub;
            _httpContext = httpContextAccessor.HttpContext;

            _hub.OnRequest += Handle;
        }

        private readonly HttpContext _httpContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRpcHub _hub;
        private string _clientId;

        /// <summary>
        /// If preset, this side will be seen a client, if null, this side will be seen as a server.
        /// </summary>
        public string ClientId { get; set; } = "";

        /// <summary>
        /// If request is notification, all matched method will be invoked, if not, the first matched method will 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RpcResponse Handle(RpcRequest request)
        {
            using (var scopedProvider = _serviceProvider.CreateScope())
            {
                var pipeline = scopedProvider.ServiceProvider.GetService<IPipeline>();
                var methodProvider = scopedProvider.ServiceProvider.GetService<IRpcMethodProvider>();
                methodProvider.Path = RoutingPath.Parse(_httpContext.Request.Path);
                var allMethods = methodProvider.GetAllMethods();
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
                var methods = allMethods.Where(m => GetMethodName(m.Prototype) == methodName).ToList();

                // insensitive case
                if (methods.Count == 0)
                    methods = allMethods
                        .Where(m => string.Equals(GetMethodName(m.Prototype), methodName,
                            StringComparison.CurrentCultureIgnoreCase)).ToList();
                if (methods.Count == 0)
                {
                    var e = new Exception("Method not found.");
                    throw new RpcException(RpcErrorCode.MethodNotFound, e.Message, e);
                }

                if (request.IsNotify())
                {
                    foreach (var method in methods)
                    {
                        pipeline.ProcessRequestAsync(method, context).Wait();
                        //_methodInvoker.Notify(method, ref context);
                    }

                    return null;
                }
                else
                {
                    var method = methods.FirstOrDefault();
                    if (method == null) return context.Response;
                
                    pipeline.ProcessRequestAsync(method, context).Wait();
                    //var res = _methodInvoker.Call(method, ref context);

                    return context.Response;
                }
            }
        }

        private string GetMethodName(MethodInfo m)
        {
            var attribute = m.GetCustomAttributes<RpcMethodAttribute>().FirstOrDefault();
            return attribute == null ? m.Name : attribute.MethodName;
        }
    }
}