using System.Collections.Generic;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Filter;

namespace BridgeRpc.AspNetCore.Router.Pipeline
{
    public class GlobalFiltersList : List<IRpcFilter>
    {
        
    }
}