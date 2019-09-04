using System;
using BridgeRpc.AspNetCore.Router;

namespace Client.RpcControllers
{
    [RpcClient]
    public class EntryController : RpcController
    {
        [RpcMethod("sayHi")]
        public string SayHi( string name)
        {
            Console.WriteLine("Hi from " + name);
            return name;
        }
    }
}