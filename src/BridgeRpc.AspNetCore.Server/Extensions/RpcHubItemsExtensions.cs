using BridgeRpc.AspNetCore.Router;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Server.Extensions
{
    public static class RpcHubItemsExtensions
    {
        public const string ItemNameRoutingPath = "__RoutingPath";

        /**
         * Get routing path.
         */
        public static RoutingPath GetRoutingPath(this IRpcHub hub)
        {
            if (hub.Items.ContainsKey(ItemNameRoutingPath))
                return (RoutingPath) hub.Items[ItemNameRoutingPath];
            return null;
        }

        /**
         * Set routing path.
         *
         * DO NOT call this function unless you know what you are doing!
         */
        public static void SetRoutingPath(this IRpcHub hub, RoutingPath path)
        {
            hub.Items[ItemNameRoutingPath] = path;
        }
    }
}