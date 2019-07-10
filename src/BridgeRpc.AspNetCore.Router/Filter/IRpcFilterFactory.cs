using System;

namespace BridgeRpc.AspNetCore.Router.Filter
{
    public interface IRpcFilterFactory
    {
        IRpcFilter CreateInstance(IServiceProvider serviceProvider);
    }
}