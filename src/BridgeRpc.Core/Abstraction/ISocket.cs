using System;

namespace BridgeRpc.Abstraction
{
    public delegate void OnReceivedEventHandler(object sender, byte[] data);
    
    public interface ISocket
    {
        /// <summary>
        /// Send data.
        /// </summary>
        /// <param name="data">Data will be transferred</param>
        void Send(byte[] data);
        
        /// <summary>
        /// Invoke when socket received data.
        /// </summary>
        event OnReceivedEventHandler OnReceived;
        
        /// <summary>
        /// Invoke when socket disconnected
        /// </summary>
        event Action<string> OnDisconnect;
        
        /// <summary>
        /// Disconnect.
        /// </summary>
        void Disconnect();
    }
}