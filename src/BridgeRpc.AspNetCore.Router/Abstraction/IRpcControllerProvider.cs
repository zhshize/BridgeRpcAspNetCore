using System.Collections.Generic;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcControllerProvider
    {
        List<RpcController> GetControllers();
    }
}