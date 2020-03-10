using System;
using BridgeRpc.Core.Abstraction;
using Microsoft.AspNetCore.Http;

namespace BridgeRpc.AspNetCore.Server
{
    public class ServerEventBus
    {
        public event Action<HttpContext, IRpcHub> OnConnected;
        public event Action<HttpContext> OnDisconnected;
        public event Action<HttpContext> OnNotAllowed;

        /// <summary>
        ///     This method shouldn't be invoke by user.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="hub"></param>
        public void InvokeConnected(HttpContext context, IRpcHub hub)
        {
            OnConnected?.Invoke(context, hub);
        }

        /// <summary>
        ///     This method shouldn't be invoke by user.
        /// </summary>
        /// <param name="context"></param>
        public void InvokeDisconnected(HttpContext context)
        {
            OnDisconnected?.Invoke(context);
        }

        /// <summary>
        ///     This method shouldn't be invoke by user.
        /// </summary>
        /// <param name="context"></param>
        public void InvokeNotAllowed(HttpContext context)
        {
            OnNotAllowed?.Invoke(context);
        }
    }
}