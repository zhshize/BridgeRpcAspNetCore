using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Pipeline
{
    /// <summary>
    ///     Pipeline of request handling
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        ///     Start pipeline processing
        /// </summary>
        /// <param name="method">Request handling method</param>
        /// <param name="context">Request handling action context</param>
        /// <returns>The <see cref="Task" /> object represents pipeline processing</returns>
        Task<IRpcActionContext> ProcessRequestAsync(IRpcMethod method, IRpcActionContext context);
    }
}