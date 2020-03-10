using System;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.AspNetCore.Router.Filter;

namespace Server
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TestingFilterAttribute : Attribute, IRpcFilterFactory
    {
        private readonly string _message;

        public TestingFilterAttribute(string message = "")
        {
            _message = message;
        }

        public IRpcFilter CreateInstance(IServiceProvider serviceProvider)
        {
            return new TestingFilter(_message);
        }
    }

    public class TestingFilter : IRpcFilter
    {
        private readonly string _message;

        public TestingFilter(string message = "")
        {
            _message = message;
        }

        public async Task OnActionExecutionAsync(IRpcActionContext context, Func<Task> next)
        {
            Console.WriteLine(_message);
            await next();
            Console.WriteLine("after " + _message);
        }
    }
}