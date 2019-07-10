using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    public class RpcFilter : IRpcFilter
    {
        private readonly Func<IRpcActionContext, Func<Task>, Task> _function;

        public RpcFilter(Func<IRpcActionContext, Func<Task>, Task> function)
        {
            _function = function;
        }

        public async Task OnActionExecutionAsync(IRpcActionContext context, Func<Task> next)
        {
            await _function(context, next);
        }
    }
}