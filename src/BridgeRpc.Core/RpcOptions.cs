using System;
using System.Collections.Generic;

namespace BridgeRpc.Core
{
    public class RpcOptions
    {
        public TimeSpan RequestTimeout { get; set; } = TimeSpan.FromSeconds(15);
        public int BufferSize { get; set; } = 16 * 1024;
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(120);
        public IList<string> AllowedOrigins { get; set; } = new List<string> { "*" };
    }
}