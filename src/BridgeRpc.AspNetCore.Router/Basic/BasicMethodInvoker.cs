using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;
using BridgeRpc.Core;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicMethodInvoker : IMethodInvoker
    {
        private readonly RpcOptions _options;

        public BasicMethodInvoker(RpcOptions options)
        {
            _options = options;
        }

        public RpcResponse Call(IRpcMethod method, ref IRpcActionContext context)
        {
            var args = GetArguments(method, context.Request);
            method.Controller.RpcContext = context;
            if (IsAsyncMethod(method))
            {
                if (method.Prototype.ReturnType == typeof(Task<RpcResponse>))
                {
                    // for async Task<RpcResponse>
                    var t = (Task<RpcResponse>) method.Prototype.Invoke(method.Controller, args);
                    var success = t.Wait(_options.RequestTimeout);

                    if (success) return t.Result;

                    var timeoutException = new TimeoutException("Call handler function timeout.");
                    throw new RpcException(RpcErrorCode.InternalError, timeoutException.Message, timeoutException);
                }
                else
                {
                    // for async Task<any>
                    var t = (dynamic) method.Prototype.Invoke(method.Controller, args);
                    var success = (bool) t.Wait(_options.RequestTimeout);

                    var res = new RpcResponse();
                    res.SetResult(t.Result);

                    if (success) return res;

                    var timeoutException = new TimeoutException("Call handler function timeout.");
                    throw new RpcException(RpcErrorCode.InternalError, timeoutException.Message, timeoutException);
                }
            }

            if (method.Prototype.ReturnType == typeof(RpcResponse))
                // for RpcResponse
                return (RpcResponse) method.Prototype.Invoke(method.Controller, args);

            if (method.Prototype.ReturnType == typeof(void))
            {
                return context.Response;
            }

            {
                // for any
                var t = method.Prototype.Invoke(method.Controller, args);

                context.Response.SetResult(t);
                return context.Response;
            }
        }

        public void Notify(IRpcMethod method, ref IRpcActionContext context)
        {
            var args = GetArguments(method, context.Request);
            method.Controller.RpcContext = context;
            method.Prototype.Invoke(method.Controller, args);
        }

        protected object[] GetArguments(IRpcMethod method, RpcRequest request)
        {
            var paramInfos = method.Prototype.GetParameters();
            var args = new List<object>();

            foreach (var p in paramInfos)
            {
                var dataAttr = p.GetCustomAttributes<RpcDataAttribute>().ToList().Count == 0
                    ? null
                    : p.GetCustomAttributes<RpcDataAttribute>().ToList()[0];

                var paramAttr = p.GetCustomAttributes<RpcParamAttribute>().ToList().Count == 0
                    ? null
                    : p.GetCustomAttributes<RpcParamAttribute>().ToList()[0];

                if (dataAttr != null)
                {
                    var data = request
                        .GetType()
                        .GetMethods()
                        .Where(x => x.Name == nameof(request.GetData))
                        .First(x => x.IsGenericMethod)?
                        .MakeGenericMethod(dataAttr.TypeRequired == null ? p.ParameterType : dataAttr.TypeRequired)
                        .Invoke(request, null);
                    args.Add(data);
                }
                else if (p.ParameterType == typeof(RpcRequest))
                {
                    args.Add(request);
                }
                else if (paramAttr != null)
                {
                    var paramName = paramAttr.ParameterName;
                    if (string.IsNullOrEmpty(paramName))
                        paramName = p.Name;

                    var getParamMethod = typeof(RpcRequest).GetMethod("GetParameterFromData");
                    if (getParamMethod == null) continue;
                    var methodRef = getParamMethod.MakeGenericMethod(p.ParameterType);
                    args.Add(methodRef.Invoke(request, new object[] {paramName}));
                }
                else
                {
                    var getParamMethod = typeof(RpcRequest).GetMethod("GetParameterFromData");
                    if (getParamMethod == null) continue;
                    var methodRef = getParamMethod.MakeGenericMethod(p.ParameterType);
                    args.Add(methodRef.Invoke(request, new object[] {p.Name}));
                }
            }

            return args.ToArray();
        }

        private bool IsAsyncMethod(IRpcMethod method)
        {
            var attrib = method.Prototype.GetCustomAttribute<AsyncStateMachineAttribute>();
            return attrib != null;
        }

        //protected MethodInfo SerializeMethod { get; }
    }
}