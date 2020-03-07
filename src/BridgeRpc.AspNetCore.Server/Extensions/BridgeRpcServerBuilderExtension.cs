using System.Threading.Tasks;
using System.Timers;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.AspNetCore.Router.Basic;
using BridgeRpc.Core.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BridgeRpc.AspNetCore.Server.Extensions
{
    public delegate void ServerEventHandler(ref ServerEventBus bus);

    public static class BridgeRpcServerBuilderExtension
    {
        /// <summary>
        ///     Directly use BridgeRpc with basic configuration. WebSocketOptions will be controlled by this method.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="handler">Register event handler</param>
        /// <returns></returns>
        public static IApplicationBuilder UseBridgeRpcWithBasic(this IApplicationBuilder app,
            ServerEventHandler handler)
        {
            var wsOptions = app.ApplicationServices.GetService<WebSocketOptions>();

            app.UseWebSockets(wsOptions);

            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var logger = context.RequestServices.GetService<ILogger>();
                    // Check path is allowed or not
                    var routingOptions = context.RequestServices.GetService<RoutingOptions>();
                    var currentPath = RoutingPath.Parse(context.Request.Path);
                    var isAllowed = routingOptions.AllowAny;
                    if (!isAllowed)
                        foreach (var path in routingOptions.AllowedPaths)
                        {
                            if (!currentPath.Equals(path)) continue;
                            isAllowed = true;
                            break;
                        }

                    var bus = new ServerEventBus();
                    handler(ref bus);

                    if (isAllowed)
                    {
                        using (var websocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            var options = context.RequestServices.GetService<RpcServerOptions>();
                            var socket = (BasicSocket) context.RequestServices.GetRequiredService<ISocket>();
                            socket.SetSocket(websocket);
                            var router = context.RequestServices.GetRequiredService<BasicRouter>();
                            var hub = context.RequestServices.GetRequiredService<IRpcHub>();

                            // ping-pong
                            var pingTimer = new Timer
                            {
                                Interval = options.PingInterval.TotalMilliseconds,
                                AutoReset = true
                            };
                            pingTimer.Elapsed += async (sender, args) =>
                            {
                                try
                                {
                                    await hub.RequestAsync(".ping", null, true, options.PongTimeout);
                                }
                                catch
                                {
                                    hub.Disconnect();
                                }
                            };
                            pingTimer.Enabled = true;

                            bus.InvokeConnected(context, hub);
                            hub.SetRoutingPath(currentPath);
                            await socket.Start();
                            bus.InvokeDisconnected(context);
                        }
                    }
                    else
                    {
                        bus.InvokeNotAllowed(context);
                        logger.LogInformation(
                            "A web socket connection is reject by BridgeRpc because the path is not allowed.");
                        await next();
                    }
                }
                else
                {
                    await next();
                }
            });

            return app;
        }

        /// <summary>
        ///     Directly use BridgeRpc with basic configuration. WebSocketOptions will be controlled by this method.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseBridgeRpcWithBasic(this IApplicationBuilder app)
        {
            return app.UseBridgeRpcWithBasic((ref ServerEventBus bus) => { });
        }

        /// <summary>
        ///     Start BridgeRpc connection in a controller. Because user maps routing directly, so this method will not
        ///     check the routing path.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<IActionResult> EnableBridgeRpc(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
                using (var websocket = await context.WebSockets.AcceptWebSocketAsync())
                {
                    var socket = (BasicSocket) context.RequestServices.GetRequiredService<ISocket>();
                    socket.SetSocket(websocket);
                    context.RequestServices.GetRequiredService<BasicRouter>();
                    await socket.Start();
                }

            return new EmptyResult();
        }
    }
}