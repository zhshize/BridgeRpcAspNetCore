using System;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    ///     Attribute to decorate a derived <see cref="RpcController" /> class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RpcRouteAttribute : Attribute
    {
        /// <summary>
        ///     Set routing path to be used in the router. If unspecified, this controller can handle request from any path.
        /// </summary>
        /// <param name="routeName">(Optional) Routing path</param>
        public RpcRouteAttribute(string routeName = null)
        {
            RouteName = routeName?.Trim();
        }

        /// <summary>
        ///     Routing path to be used in the router. If unspecified, this controller can handle request from any path.
        /// </summary>
        public string RouteName { get; }
    }
}