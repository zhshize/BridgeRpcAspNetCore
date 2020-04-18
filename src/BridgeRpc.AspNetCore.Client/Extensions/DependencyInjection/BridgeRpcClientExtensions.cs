using System.Linq;
using System.Reflection;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Basic;
using BridgeRpc.AspNetCore.Router.Pipeline;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace BridgeRpc.AspNetCore.Client.Extensions.DependencyInjection
{
    public static class BridgeRpcClientExtensions
    {
        public delegate void OptionsProvider(ref RpcClientOptions options);

        public static void AddBridgeRpcClient(this IServiceCollection services, OptionsProvider optionProvider)
        {
            services.AddHttpContextAccessor();

            var o = new RpcClientOptions();
            optionProvider(ref o);

            services.AddScoped<SocketProvider>();

            services.AddScoped(provider => o.RpcOptions);
            services.AddScoped(provider => o);

            services.AddScoped<GlobalFiltersList>();
            services.AddScoped<Connection>();
            services.AddScoped<ISocket, BasicSocket>(provider =>
                (BasicSocket) provider.GetService<SocketProvider>().Socket);
            services.AddScoped<IRpcHub, RpcHub>();
            services.AddScoped<BasicRouter>();
            services.AddScoped<IMethodInvoker, BasicMethodInvoker>();
            services.AddScoped<IRpcControllerProvider, BasicRpcControllerProvider>();
            services.AddScoped<IRpcMethodProvider, BasicRpcMethodProvider>();
            services.AddScoped<IRpcActionContext, RpcActionContext>();

            AddRpcControllers(services);
        }

        public static void AddBridgeRpcClient(this IServiceCollection services)
        {
            services.AddBridgeRpcClient((ref RpcClientOptions options) => { });
        }

        public static void AddRpcControllers(IServiceCollection services)
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