﻿using System;
using System.Collections.Generic;
using BridgeRpc.AspNetCore.Client;
using BridgeRpc.AspNetCore.Client.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Client
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
            
            services.AddBridgeRpcClient((ref RpcClientOptions options) =>
            {
                options.Host = new Uri("ws://localhost/");
                options.ClientId = "client1";
                options.ReconnectInterval = TimeSpan.FromSeconds(60);
                
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

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}