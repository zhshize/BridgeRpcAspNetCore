using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgeRpc;
using BridgeRpc.Abstraction;
using BridgeRpc.AspNetCore.Router;
using MessagePack;

namespace Server.RpcControllers
{
    [RpcRoute("/go")]
    public class EntryController : RpcController
    {
        private readonly IRpcHub _rpcHub;

        public EntryController(IRpcHub rpcHub)
        {
            _rpcHub = rpcHub;
        }

        [RpcMethod("greet")]
        public async Task<string> Greet(RpcRequest request)
        {
            var friend = request.GetData<string>();
            
            Console.WriteLine("Got greet from " + friend);

            var data = new Dictionary<string, string>();
            data.Add("name", "amy");

            RpcResponse reqTask = await _rpcHub.RequestAsync("sayHi",
                MessagePackSerializer.Serialize(data));
            Console.WriteLine("Amy says Hi, got reply, he says " + reqTask.GetResult<string>());

            return "Greeted";
        }
    }
}