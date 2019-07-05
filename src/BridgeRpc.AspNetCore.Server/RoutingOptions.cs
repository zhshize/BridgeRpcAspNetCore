using System.Collections.Generic;
using BridgeRpc.AspNetCore.Router;

namespace BridgeRpc.AspNetCore.Server
{
    public class RoutingOptions
    {
        public bool AllowAny { get; set; } = false;
        public List<RoutingPath> AllowedPaths { get; set; } = new List<RoutingPath>();
    }
}