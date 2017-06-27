using System;

namespace ClientServerUsingNamedPipes.Interfaces
{
    /// <summary>
    /// Interface ICommunicationServer
    /// </summary>
    /// <seealso cref="ClientServerUsingNamedPipes.Interfaces.ICommunication" />
    public interface ICommunicationServer : ICommunication
    {
        /// <summary>
        /// The server id
        /// </summary>
        string ServerId { get; }

        /// <summary>
        /// This event is fired when a message is received
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> MessageReceivedEvent;

        /// <summary>
        /// This event is fired when a client connects
        /// </summary>
        event EventHandler<ClientConnectedEventArgs> ClientConnectedEvent;

        /// <summary>
        /// This event is fired when a client disconnects
        /// </summary>
        event EventHandler<ClientDisconnectedEventArgs> ClientDisconnectedEvent;
    }

    /// <summary>
    /// Class ClientConnectedEventArgs.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ClientConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; }
    }

    /// <summary>
    /// Class ClientDisconnectedEventArgs.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ClientDisconnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; }
    }

    /// <summary>
    /// Class MessageReceivedEventArgs.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }
    }
}