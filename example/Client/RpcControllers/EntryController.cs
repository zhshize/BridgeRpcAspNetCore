using System;
using BridgeRpc.AspNetCore.Router;

namespace Client.RpcControllers
{
    [RpcClient]
    public class EntryController : RpcController
    {
        [RpcMethod("sayHi")]
        public string SayHi([RpcParam("name")] string n)
        {
            Console.WriteLine("Hi from " + n);
            return n;
        }
    }
}