using System.Threading.Tasks;

namespace BridgeRpc.Core.Util
{
    public class RequestTaskCompletionSource : TaskCompletionSource<RpcResponse>
    {
        public bool ThrowRpcException { get; set; }
    }
}