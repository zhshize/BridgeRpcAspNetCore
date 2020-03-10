using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Filter;

namespace BridgeRpc.AspNetCore.Router.Pipeline
{
    public class Pipeline : IPipeline
    {
        private readonly GlobalFiltersList _globalFiltersList;
        private readonly IMethodInvoker _methodInvoker;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     Create pipeline to handle request with filters
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <param name="globalFiltersList">Global filters</param>
        /// <param name="methodInvoker">Method invoker object</param>
        public Pipeline(IServiceProvider serviceProvider, GlobalFiltersList globalFiltersList,
            IMethodInvoker methodInvoker)
        {
            _serviceProvider = serviceProvider;
            _globalFiltersList = globalFiltersList;
            _methodInvoker = methodInvoker;
        }

        public async Task<IRpcActionContext> ProcessRequestAsync(IRpcMethod method, IRpcActionContext context)
        {
            var filters = new List<IRpcFilter>();

            filters.AddRange(_globalFiltersList);

            // for directly applied filters from controller
            filters.AddRange(method.Controller.GetType()
                .GetCustomAttributes()
                .OfType<IRpcFilter>());

            // for IRpcFilterFactory from controller
            filters.AddRange(method.Controller.GetType()
                .GetCustomAttributes()
                .OfType<IRpcFilterFactory>()
                .Select(a => a.CreateInstance(_serviceProvider)));

            // for directly applied filters from method
            filters.AddRange(method.Prototype
                .GetCustomAttributes()
                .OfType<IRpcFilter>());

            // for IRpcFilterFactory from method
            filters.AddRange(method.Prototype
                .GetCustomAttributes()
                .OfType<IRpcFilterFactory>()
                .Select(a => a.CreateInstance(_serviceProvider)));

            filters.Add(new RpcFilter(async (actionContext, next) =>
            {
                if (actionContext.Request.IsNotify())
                {
                    _methodInvoker.Notify(method, ref actionContext);
                }
                else
                {
                    var res = _methodInvoker.Call(method, ref actionContext);
                    if (res != null) actionContext.Response = res;
                }

                await next();
            }));

            await CallFilters(context, filters);

            return context;
        }

        /// <summary>
        ///     Call filters
        /// </summary>
        /// <param name="context">Request handling action context</param>
        /// <param name="filters">Filters to be called</param>
        /// <returns>The <see cref="Task" /> object represent the filter calling process</returns>
        protected async Task CallFilters(IRpcActionContext context, List<IRpcFilter> filters)
        {
            var enumerator = filters.GetEnumerator();
            if (!enumerator.MoveNext()) return;

            async Task NextFilter()
            {
                if (enumerator.MoveNext()) await enumerator.Current.OnActionExecutionAsync(context, NextFilter);
            }

            await filters[0].OnActionExecutionAsync(context, NextFilter);
        }
    }
}