using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    /// <summary>
    /// Function-based RPC calling filters
    /// </summary>
    public class RpcFilter : IRpcFilter
    {
        private readonly Func<IRpcActionContext, Func<Task>, Task> _function;

        /// <summary>
        /// Create a filter by calling the function passed
        /// </summary>
        /// <param name="function">filtering function</param>
        public RpcFilter(Func<IRpcActionContext, Func<Task>, Task> function)
        {
            _function = function;
        }

        /// <summary>
        /// Call the filtering function
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(IRpcActionContext context, Func<Task> next)
        {
            await _function(context, next);
        }
    }
}