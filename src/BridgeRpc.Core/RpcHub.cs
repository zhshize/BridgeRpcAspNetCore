using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BridgeRpc.Core.Abstraction;
using BridgeRpc.Core.Util;
using Newtonsoft.Json.Linq;

namespace BridgeRpc.Core
{
    public class RpcHub : IRpcHub
    {
        private readonly ISocket _socket;

        public RpcHub(ISocket socket)
        {
            _socket = socket;
            _socket.OnReceived += Handle;
            _socket.OnDisconnect += s => OnDisconnect?.Invoke();
        }

        public Dictionary<string, object> Items { get; } = new Dictionary<string, object>();
        public event RequestHandler OnRequest;
        public event DisconnectHandler OnDisconnect;
        public event MessageExceptionHandler OnMessageException;
        public event RequestInvokingExceptionHandler OnRequestInvokingException;

        public Task<RpcResponse> RequestAsync(string method, object param, bool throwRpcException = false, TimeSpan? timeout = null)
        {
            return RequestAsync(method, param, timeout.HasValue, throwRpcException, timeout);
        }

        protected Task<RpcResponse> RequestAsync(string method, object param, bool hasTimeout, bool throwRpcException, TimeSpan? timeout)
        {
            var id = Util.Util.RandomString(16);
            var request = new RpcRequest
            {
                Id = id,
                Method = method,
            };
            request.SetData(param);

            var taskSource = new RequestTaskCompletionSource { ThrowRpcException = throwRpcException };
            var task = taskSource.Task;
            RequestingQueue.Add(id, taskSource);

            try
            {
                _socket.Send(Encoding.UTF8.GetBytes(request.ToJson()));
            }
            catch (Exception e)
            {
                taskSource.SetException(e);
            }

            if (hasTimeout && timeout != null)
                Task.Run(() => 
                        Task.Delay(timeout.Value)
                        .ContinueWith(_ => taskSource.TrySetCanceled()));

            return task;
        }

        public void Notify(string method, object param)
        {
            var request = new RpcRequest
            {
                Id = null,
                Method = method,
            };
            request.SetData(param);
            _socket.Send(Encoding.UTF8.GetBytes(request.ToJson()));
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        protected readonly Dictionary<string, RequestTaskCompletionSource> RequestingQueue
            = new Dictionary<string, RequestTaskCompletionSource>();

        protected void Handle(object sender, byte[] data)
        {
            Task.Run(() => // prevent from blocking thread
            {
                string json = null;
                string method = null;
                try
                {
                    json = Encoding.UTF8.GetString(data);
                    var jo = JObject.Parse(json);
                    if (jo.Type == JTokenType.Object)
                    {
                        var mp = jo.Property("method");
                        if (mp != null && mp.Type == JTokenType.Property && mp.Value.Type == JTokenType.String)
                            method = mp.Value.Value<string>();
                    }
                }
                catch (ArgumentOutOfRangeException ae)
                {
                    OnMessageException?.Invoke(ae, "data index is invalid in Handle(object sender, byte[] data)");
                    return;
                }
                catch (ArgumentNullException ae)
                {
                    OnMessageException?.Invoke(ae, "data is null in Handle(object sender, byte[] data)");
                    return;
                }
                catch (DecoderFallbackException de)
                {
                    OnMessageException?.Invoke(de, "Decoding to UTF-8 string failed in Handle(object sender, byte[] data)");
                    return;
                }
                catch (ArgumentException ae)
                {
                    OnMessageException?.Invoke(ae, "Decoding to UTF-8 string failed in Handle(object sender, byte[] data)");
                    return;
                }
                catch (KeyNotFoundException)
                {
                    method = null;
                }
                catch (Exception e)
                {
                    OnMessageException?.Invoke(e, "Other internal error");
                    return;
                }

                if (method != null) // Received data is request
                {
                    RpcRequest req;
                    try
                    {
                        req = new RpcRequest(json);
                    }
                    catch (Exception e)
                    {
                        // because request cannot be deserialized, no error object will be sent back
                        OnMessageException?.Invoke(e, e.Message);
                        return;
                    }

                    try
                    {
                        var res = OnRequest?.Invoke(req);
                        if (req.IsNotify() || res == null) return;
                        res.Id = req.Id;
                        _socket.Send(Encoding.UTF8.GetBytes(res.ToJson()));
                    }
                    catch (RpcException e)
                    {
                        try // Send error message back
                        {
                            var res = new RpcResponse
                            {
                                Id = req.Id,
                            };
                            res.SetError<object>(e.ErrorCode, e.Message, e.RpcData);
                            _socket.Send(Encoding.UTF8.GetBytes(res.ToJson()));
                        }
                        catch (Exception inner)
                        {
                            // error in error handling, Response will NOT be sent
                            OnMessageException?.Invoke(new AggregateException(e, inner), 
                                "Error occured inside error handling in RpcHub.Handle(...)");
                        }
                        OnRequestInvokingException?.Invoke(e, e.Message);
                    }
                    catch (Exception e)
                    {
                        try // Send error message back
                        {
                            var res = new RpcResponse
                            {
                                Id = req.Id,
                            };
                            res.SetError<object>((int) RpcErrorCode.InternalError, e.Message, null);
                            _socket.Send(Encoding.UTF8.GetBytes(res.ToJson()));
                        }
                        catch (Exception inner)
                        {
                            // error in error handling, Response will NOT be sent
                            OnMessageException?.Invoke(new AggregateException(e, inner), 
                                "Error occured inside error handling in RpcHub.Handle(...)");
                        }
                        OnRequestInvokingException?.Invoke(e, e.Message);
                    }
                }
                else // Received data is response
                {
                    RpcResponse response;
                    try
                    {
                        response = new RpcResponse(json);
                    }
                    catch (Exception e)
                    {
                        var rpcE = new RpcException(RpcErrorCode.ParseError, "Parsing response object error.", e);
                        OnMessageException?.Invoke(rpcE, rpcE.Message);
                        return;
                    }
                    
                    if (response.Id != null && RequestingQueue.ContainsKey(response.Id))
                    {
                        RequestingQueue[response.Id].SetResult(response);
                    }
                }
            });
        }
    }
}