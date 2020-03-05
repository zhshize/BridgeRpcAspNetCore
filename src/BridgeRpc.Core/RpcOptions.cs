using System;
using System.Collections.Generic;

namespace BridgeRpc.Core
{
    /// <summary>
    /// Options for RPC connection
    /// </summary>
    public class RpcOptions
    {
        /// <summary>
        /// Timeout for processing request. (default 15 seconds)
        /// </summary>
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);
        
        /// <summary>
        /// Buffer size for socket connection. (default 16 kb)
        /// </summary>
        public int BufferSize { get; set; } = 16 * 1024;
        
        /// <summary>
        /// The frequency at which to send Ping/Pong keep-alive control frames. (default 120 seconds)
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(120);
        
        /// <summary>
        /// Browser allowed cross site origins. (default [ "*" ])
        /// </summary>
        public IList<string> AllowedOrigins { get; set; } = new List<string> { "*" };
    }
}