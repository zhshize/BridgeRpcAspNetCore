using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BridgeRpc.AspNetCore.Router.Abstraction;
using MessagePack;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicMethodInvoker : IMethodInvoker
    {
        private readonly RpcOptions _options;

        public BasicMethodInvoker(RpcOptions options)
        {
            _options = options;
            /*SerializeMethod = typeof(MessagePackSerializer)
                .GetMethods()
                .First(m => m.Name == nameof(MessagePackSerializer.Serialize) &&
                            m.IsGenericMethod &&
                            m.GetParameters().Length == 1);*/
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
                    var finalType = method.Prototype.ReturnType.GetGenericArguments().First();

                    var t = (dynamic) method.Prototype.Invoke(method.Controller, args);
                    var success = (bool) t.Wait(_options.RequestTimeout);

                    //var serializer = SerializeMethod.MakeGenericMethod(finalType);

                    //var binary = (byte[]) serializer.Invoke(this, new [] {t.Result});
                    var res = new RpcResponse
                    {
                        Result = t.Result
                    };

                    if (success) return res;

                    var timeoutException = new TimeoutException("Call handler function timeout.");
                    throw new RpcException(RpcErrorCode.InternalError, timeoutException.Message, timeoutException);
                }
            }
            else
            {
                if (method.Prototype.ReturnType == typeof(RpcResponse))
                {
                    // for RpcResponse
                    return (RpcResponse) method.Prototype.Invoke(method.Controller, args);
                }
                else if (method.Prototype.ReturnType == typeof(void))
                {
                    return context.Response;
                }
                else
                {
                    // for any
                    var finalType = method.Prototype.ReturnType;
                    var t = method.Prototype.Invoke(method.Controller, args);

                    //var serializer = SerializeMethod.MakeGenericMethod(finalType);

                    //var binary = (byte[]) serializer.Invoke(this, new [] {t});
                    return new RpcResponse
                    {
                        Result = t
                    };
                }
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
                    var data = request.GetType().GetMethod(nameof(request.GetData))
                        .MakeGenericMethod(dataAttr.TypeRequired).Invoke(request, null);
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
                    args.Add(request.GetParameterFromData(paramName));
                }
                else
                {
                    args.Add(request.GetParameterFromData(p.Name));
                }
            }

            return args.ToArray();
        }

        protected static bool IsAsyncMethod(IRpcMethod method)
        {
            var attrib = method.Prototype.GetCustomAttribute<AsyncStateMachineAttribute>();
            return attrib != null;
        }

        //protected MethodInfo SerializeMethod { get; }
    }
}