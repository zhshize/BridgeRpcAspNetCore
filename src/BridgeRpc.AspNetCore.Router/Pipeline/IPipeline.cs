using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Pipeline
{
    public interface IPipeline
    {
        Task<IRpcActionContext> ProcessRequestAsync(IRpcMethod method, IRpcActionContext context);
    }
}