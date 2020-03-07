using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    /// <summary>
    ///     RPC calling filter.
    /// </summary>
    public interface IRpcFilter
    {
        /// <summary>
        ///     Filtering the request
        /// </summary>
        /// <param name="context">Request handling action context</param>
        /// <param name="next">
        ///     The next filter or handler to be called, if this function not called, this request
        ///     will be blocked.
        /// </param>
        /// <returns>The <see cref="Task" /> object represent the filtering process</returns>
        Task OnActionExecutionAsync(IRpcActionContext context, Func<Task> next);
    }
}