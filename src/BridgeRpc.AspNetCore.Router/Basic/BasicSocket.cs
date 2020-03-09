using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using BridgeRpc.Core;
using BridgeRpc.Core.Abstraction;

namespace BridgeRpc.AspNetCore.Router.Basic
{
    public class BasicSocket : ISocket
    {
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private readonly RpcOptions _options;

        /// <summary>
        ///     Concurrent queue for messages will be sent
        /// </summary>
        protected readonly ConcurrentQueue<byte[]> SendQueue;

        private WebSocket _socket;

        public BasicSocket(RpcOptions options)
        {
            _options = options;
            SendQueue = new ConcurrentQueue<byte[]>();
        }

        public event OnReceivedEventHandler OnReceived;
        public event Action<string> OnDisconnect;

        public void Send(byte[] data)
        {
            SendQueue.Enqueue(data);
        }

        public void Disconnect()
        {
            DisconnectAsync().Wait();
        }

        public void SetSocket(WebSocket socket)
        {
            _socket = socket;
        }

        /// <summary>
        ///     Start data transfer.
        /// </summary>
        /// <returns>When socket closed</returns>
        public async Task Start()
        {
            if (_cancellation.IsCancellationRequested) return;
            var send = Task.Run(StartSend);

            WebSocketReceiveResult closeResult = null;
            var closeStatus = WebSocketCloseStatus.NormalClosure;

            while (!_cancellation.IsCancellationRequested)
                try
                {
                    var (result, message) = await ReceiveFullMessage();
                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        var array = message.ToArray();
                        var memStream = new MemoryStream(array, 0, array.Length);
                        try
                        {
                            OnReceived?.Invoke(this, memStream.ToArray());
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        closeResult = result;
                        break;
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                {
                    closeStatus = WebSocketCloseStatus.ProtocolError;
                    break;
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine("Receive error: ");
                    Console.WriteLine(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Other error when receiving: ");
                    Console.WriteLine(e);
                }

            _cancellation.Cancel();
            await send;
            if (closeResult?.CloseStatus != null) closeStatus = (WebSocketCloseStatus) closeResult.CloseStatus;
            await DisconnectAsync(closeStatus, closeResult?.CloseStatusDescription);
        }

        private async Task<(WebSocketReceiveResult, IEnumerable<byte>)> ReceiveFullMessage()
        {
            WebSocketReceiveResult response;
            var message = new List<byte>();

            var buffer = new byte[_options.BufferSize];
            do
            {
                response = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellation.Token);
                message.AddRange(new ArraySegment<byte>(buffer, 0, response.Count));
            } while (!response.EndOfMessage);

            return (response, message);
        }

        /// <summary>
        ///     Send data when <see cref="SendQueue">_sendQueue</see> has data.
        /// </summary>
        /// <returns></returns>
        protected async Task StartSend()
        {
            while (!_cancellation.IsCancellationRequested)
                try
                {
                    if (SendQueue.TryDequeue(out var message))
                    {
                        var sendBuffer = new ArraySegment<byte>(message, 0, message.Length);

                        await _socket.SendAsync(sendBuffer, WebSocketMessageType.Binary, true,
                            _cancellation.Token);
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(20));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (WebSocketException e) when (e.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
                {
                    try
                    {
                        OnDisconnect?.Invoke("ConnectionClosedPrematurely");
                    }
                    catch
                    {
                        // ignored
                    }

                    break;
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine("Send error: ");
                    Console.WriteLine(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Other error when sending: ");
                    Console.WriteLine(e);
                }
        }

        /// <summary>
        ///     Disconnect this socket.
        /// </summary>
        /// <param name="closeStatus"></param>
        /// <param name="statusDescription"></param>
        /// <returns></returns>
        public async Task DisconnectAsync(
            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
            string statusDescription = "")
        {
            try
            {
                await _socket.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
                _cancellation.Cancel();
            }
            catch (Exception)
            {
                // Exit normally
            }

            try
            {
                OnDisconnect?.Invoke(statusDescription);
            }
            catch
            {
                // ignored
            }
        }
    }
}