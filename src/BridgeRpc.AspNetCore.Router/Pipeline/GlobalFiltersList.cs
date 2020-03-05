using System.Collections.Generic;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Filter;

namespace BridgeRpc.AspNetCore.Router.Pipeline
{
    /// <summary>
    /// Global RPC Calling filters
    /// </summary>
    public class GlobalFiltersList : List<IRpcFilter>
    {
        
    }
}