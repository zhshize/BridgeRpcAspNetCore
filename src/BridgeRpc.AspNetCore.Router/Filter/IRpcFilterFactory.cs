using System;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    /// <summary>
    ///     RPC calling filter factory
    ///     This class is for custom filter need custom attribute name, the usage example is in the example projects
    /// </summary>
    public interface IRpcFilterFactory
    {
        /// <summary>
        ///     Create a RPC calling filter
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>a RPC calling filter</returns>
        IRpcFilter CreateInstance(IServiceProvider serviceProvider);
    }
}