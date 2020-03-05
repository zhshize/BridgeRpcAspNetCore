using System.Linq;
using System.Reflection;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Basic;
using BridgeRpc.AspNetCore.Router.Pipeline;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Server.Extensions.DependencyInjection
{
    public static class BridgeRpcServerExtensions
    {
        public delegate void OptionsProvider(ref RpcServerOptions options);
        
        public static void AddBridgeRpc(this IServiceCollection services, OptionsProvider optionProvider)
        {
            services.AddHttpContextAccessor();
            
            var o = new RpcServerOptions();
            optionProvider(ref o);

            var wsOptions = new WebSocketOptions
            {
                KeepAliveInterval = o.RpcOptions.KeepAliveInterval,
                ReceiveBufferSize = o.RpcOptions.BufferSize
            };

            foreach (var origin in o.RpcOptions.AllowedOrigins)
            {
                wsOptions.AllowedOrigins.Add(origin);
            }

            services.AddSingleton<WebSocketOptions>(provider => wsOptions);
            
            services.AddScoped<RpcOptions>(provider => o.RpcOptions);
            services.AddScoped<RpcServerOptions>(provider => o);
            services.AddScoped<RoutingOptions>(provider => o.RoutingOptions);
            
            services.AddScoped<ISocket, BasicSocket>();
            services.AddScoped<GlobalFiltersList>();
            services.AddScoped<IPipeline, Pipeline>();
            services.AddScoped<IRpcHub, RpcHub>();
            services.AddScoped<BasicRouter>();
            services.AddScoped<IMethodInvoker, BasicMethodInvoker>();
            services.AddScoped<IRpcControllerProvider, BasicRpcControllerProvider>();
            services.AddScoped<IRpcMethodProvider, BasicRpcMethodProvider>();
            services.AddScoped<IRpcActionContext, RpcActionContext>();

            AddRpcControllers(services);
        }
        
        public static void AddBridgeRpc(this IServiceCollection services)
        {
            services.AddBridgeRpc((ref RpcServerOptions options) => { });
        }

        private static void AddRpcControllers(IServiceCollection services)
        {
            var controllerTypes = Assembly.GetEntryAssembly()
                ?.DefinedTypes
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(RpcController)))
                .ToList();

            if (controllerTypes == null) return;

            foreach (var controllerType in controllerTypes)
            {
                var addService = typeof(ServiceCollectionServiceExtensions)
                    .GetMethods()
                    .First(m => m.Name == nameof(ServiceCollectionServiceExtensions.AddScoped) && 
                                m.IsGenericMethod &&
                                m.GetGenericArguments().Length == 2)
                    .MakeGenericMethod(typeof(RpcController), controllerType);
                
                addService.Invoke(null, new object[] {services});
            }
        }
    }
}