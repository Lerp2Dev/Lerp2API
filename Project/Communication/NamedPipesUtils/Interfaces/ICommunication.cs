namespace ClientServerUsingNamedPipes.Interfaces
{
    /// <summary>
    /// Interface ICommunication
    /// </summary>
    public interface ICommunication
    {
        /// <summary>
        /// Starts the communication channel
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the communication channel
        /// </summary>
        void Stop();
    }
}