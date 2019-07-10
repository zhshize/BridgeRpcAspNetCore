using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    public interface IRpcFilter
    {
        Task OnActionExecutionAsync(IRpcActionContext context, Func<Task> next);
    }
}