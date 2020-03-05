using System;

namespace BridgeRpc.Core.Abstraction
{
    /// <summary>
    /// Event handler for new data received
    /// </summary>
    /// <param name="sender">Event source</param>
    /// <param name="data">received data in bytes</param>
    public delegate void OnReceivedEventHandler(object sender, byte[] data);
    
    /// <summary>
    /// A socket for Bridge RPC data transportation
    /// </summary>
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