using System.Collections.Generic;
using System.Reflection;

namespace BridgeRpc.AspNetCore.Router.Abstraction
{
    public interface IRpcControllerPublicMethodProvider
    {
        List<MethodInfo> GetAllMethods();
    }
}