using System.Collections.Generic;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcMethodProvider
    {
        string ClientId { get; set; }
        RoutingPath Path { get; set; }
        List<IRpcMethod> GetAllMethods();
    }
}