using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router;
using BridgeRpc.Core;
using Client;

namespace Server.RpcControllers
{
    [RpcRoute]
    [TestingFilter("before action filter: 1 at EntryController")]
    [TestingFilter("before action filter: 2 at EntryController")]
    public class EntryController : RpcController
    {
        [RpcMethod("greet")]
        [TestingFilter("before action filter: 3 at Method")]
        [TestingFilter("before action filter: 4 at Method")]
        public async Task<string> Greet([RpcData] Dto request)
        {
            var friend = request.Name;

            Console.WriteLine("Got greet from " + friend);

            try
            {
                var data = new {name = "amy"};

                var reqTask = await RpcContext.Hub.RequestAsync("sayHi", data);

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