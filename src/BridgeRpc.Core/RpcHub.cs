using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BridgeRpc.Abstraction;
using MessagePack;
using MessagePack.Resolvers;

namespace BridgeRpc
{
    public class RpcHub : IRpcHub
    {
        private readonly ISocket _socket;

        public RpcHub(ISocket socket)
        {
            _socket = socket;
            _socket.OnReceived += Handle;
        }

        public event RequestHandler OnRequest;

        public Task<RpcResponse> RequestAsync(string method, object data)
        {
            return RequestAsync(method, data, false, null);
        }

        public Task<RpcResponse> RequestAsync(string method, object data, TimeSpan timeout)
        {
            return RequestAsync(method, data, true, timeout);
        }

        protected Task<RpcResponse> RequestAsync(string method, object data, bool hasTimeout, TimeSpan? timeout)
        {
            var id = Util.Util.RandomString(16);
            var request = new RpcRequest
            {
                Id = id,
                Method = method,
                Data = data
            };

            var taskSource = new TaskCompletionSource<RpcResponse>();
            var task = taskSource.Task;
            RequestingQueue.Add(id, taskSource);
            _socket.Send(request.ToBinary());

            if (hasTimeout && timeout != null)
                Task.Run(() => 
                        Task.Delay(timeout.Value)
                        .ContinueWith(_ => taskSource.TrySetCanceled()));

            return task;
        }

        public void Notify(string method, object data)
        {
            var request = new RpcRequest
            {
                Id = null,
                Method = method,
                Data = data
            };
            _socket.Send(request.ToBinary());
        }

        public void Disconnect()
        {
            _socket.Disconnect();
        }

        protected readonly Dictionary<string, TaskCompletionSource<RpcResponse>> RequestingQueue
            = new Dictionary<string, TaskCompletionSource<RpcResponse>>();

        protected void Handle(object sender, byte[] data)
        {
            Task.Run(() => // prevent from blocking thread
            {
                var dynamicModel =
                    MessagePackSerializer.Deserialize<dynamic>(data, ContractlessStandardResolver.Instance);
                string method;
                try
                {
                    method = dynamicModel["method"];
                }
                catch (KeyNotFoundException)
                {
                    method = null;
                }

                if (method != null) // Received data is request
                {
                    RpcRequest req;
                    try
                    {
                        req = MessagePackSerializer.Deserialize<RpcRequest>(data);
                    }
                    catch (Exception e)
                    {
                        // because request cannot be deserialized, no error object will be sent back
                        throw new RpcException(RpcErrorCode.ParseError, "Parsing request object error.", e);
                    }

                    try
                    {
                        var res = OnRequest?.Invoke(req);
                        if (!req.IsNotify() && res != null)
                        {
                            res.Id = req.Id;
                            _socket.Send(res.ToBinary());
                        }
                    }
                    catch (Exception e)
                    {
                        try // Send error message back
                        {
                            var res = new RpcResponse
                            {
                                Id = req.Id,
                                Error = new RpcError((int) RpcErrorCode.InternalError, e.Message, null)
                            };
                            _socket.Send(res.ToBinary());
                        }
                        catch
                        {
                            // ignored
                        }

                        throw new RpcException(RpcErrorCode.InternalError, e.Message, e);
                    }
                }
                else // Received data is response
                {
                    RpcResponse response;
                    try
                    {
                        response = MessagePackSerializer.Deserialize<RpcResponse>(data);
                    }
                    catch (Exception e)
                    {
                        throw new RpcException(RpcErrorCode.ParseError, "Parsing response object error.", e);
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