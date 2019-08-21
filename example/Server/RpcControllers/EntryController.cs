using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace Server.RpcControllers
{
    [RpcRoute]
    [TestingFilter("before action filter: 1 at EntryController")]
    [TestingFilter("before action filter: 2 at EntryController")]
    public class EntryController : RpcController
    {
        private readonly IRpcHub _rpcHub;

        public EntryController(IRpcHub rpcHub)
        {
            _rpcHub = rpcHub;
        }


        [RpcMethod("greet")]
        [TestingFilter("before action filter: 3 at Method")]
        [TestingFilter("before action filter: 4 at Method")]
        public async Task<string> Greet(RpcRequest request)
        {
            var friend = request.GetData<string>();

            Console.WriteLine("Got greet from " + friend);

            try
            {
                var data = new Dictionary<string, string> {{"name", "amy"}};

                RpcResponse reqTask = await _rpcHub.RequestAsync("sayHi", data);

                Console.WriteLine("Amy says Hi, got reply, he says " + reqTask.GetResult<string>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            return "Greeted";
        }
    }
}