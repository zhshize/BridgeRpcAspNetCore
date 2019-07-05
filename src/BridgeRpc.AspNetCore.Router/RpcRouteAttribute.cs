using System;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    /// Attribute to decorate a derived <see cref="RpcController"/> class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class RpcRouteAttribute : Attribute
    {
        /// <summary>
        /// Name of the route to be used in the router. If unspecified, will use controller name.
        /// </summary>
        public string RouteName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeName">(Optional) Name of the route to be used in the router. If unspecified, will use controller name.</param>
        /// <param name="routeGroup">(Optional) Name of the group the route is in to allow route filtering per request.</param>
        public RpcRouteAttribute(string routeName = null)
        {
            RouteName = routeName?.Trim();
        }
    }
}