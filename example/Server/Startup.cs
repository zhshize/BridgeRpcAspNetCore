using System;
using System.Collections.Generic;
using BridgeRpc;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.AspNetCore.Server;
using BridgeRpc.AspNetCore.Server.Extensions;
using BridgeRpc.AspNetCore.Server.Extensions.DependencyInjection;
using MessagePack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddBridgeRpc((ref RpcServerOptions options) =>
            {
                options.RoutingOptions.AllowAny = true;
                options.RoutingOptions.AllowedPaths = new List<RoutingPath>();
                
                options.RpcOptions.AllowedOrigins = new List<string> {"*"};
                options.RpcOptions.BufferSize = 16 * 1024;
                options.RpcOptions.KeepAliveInterval = TimeSpan.FromSeconds(120);
                options.RpcOptions.RequestTimeout = TimeSpan.FromSeconds(15);
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseBridgeRpcWithBasic((ref ServerEventBus bus) =>
            {
                bus.OnConnected += (context, hub) =>
                {
                    Console.WriteLine("Connected");
                    //hub.Notify("notify", MessagePackSerializer.Serialize("hi"));
                    hub.OnDisconnect += () => Console.WriteLine("OnDisconnect event from RpcHub.");
                };
                bus.OnNotAllowed += context => Console.WriteLine("Server not allowed this path: " + context.Request.Path);
                bus.OnDisconnected += _ => Console.WriteLine("Disconnected");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}