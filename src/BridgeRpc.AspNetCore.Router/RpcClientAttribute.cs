using System;

namespace BridgeRpc.AspNetCore.Router
{
    /// <summary>
    ///     Attribute to decorate a derived client <see cref="RpcController" /> class to handle request from server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RpcClientAttribute : Attribute
    {
        /// <summary>
        ///     Set the client id to handle specified requests.
        /// </summary>
        /// <param name="clientId">(Optional) Client id to be used in the controller.</param>
        public RpcClientAttribute(string clientId = null)
        {
            ClientId = clientId?.Trim();
        }

        /// <summary>
        ///     Client id to figure out which connection will be handled. If unspecified, any connection will be handled by
        ///     this controller.
        /// </summary>
        public string ClientId { get; }
    }
}