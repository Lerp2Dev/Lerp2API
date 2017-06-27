using ClientServerUsingNamedPipes.Interfaces;
using ClientServerUsingNamedPipes.Utilities;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerUsingNamedPipes.Client
{
    /// <summary>
    /// Class PipeClient.
    /// </summary>
    /// <seealso cref="ClientServerUsingNamedPipes.Interfaces.ICommunicationClient" />
    public class PipeClient : ICommunicationClient
    {
        #region private fields

        private readonly NamedPipeClientStream _pipeClient;

        #endregion private fields

        #region c'tor

        /// <summary>
        /// Initializes a new instance of the <see cref="PipeClient"/> class.
        /// </summary>
        /// <param name="serverId">The server identifier.</param>
        public PipeClient(string serverId)
        {
            _pipeClient = new NamedPipeClientStream(".", serverId, PipeDirection.InOut, PipeOptions.Asynchronous);
        }

        #endregion c'tor

        #region ICommunicationClient implementation

        /// <summary>
        /// Starts the client. Connects to the server.
        /// </summary>
        public void Start()
        {
            const int tryConnectTimeout = 5 * 60 * 1000; // 5 minutes
            _pipeClient.Connect(tryConnectTimeout);
        }

        /// <summary>
        /// Stops the client. Waits for pipe drain, closes and disposes it.
        /// </summary>
        public void Stop()
        {
            try
            {
                _pipeClient.WaitForPipeDrain();
            }
            finally
            {
                _pipeClient.Close();
                _pipeClient.Dispose();
            }
        }

        /// <summary>
        /// This method sends the given message asynchronously over the communication channel
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A task of TaskResult</returns>
        /// <exception cref="IOException">pipe is not connected</exception>
        public Task<TaskResult> SendMessage(string message)
        {
            var taskCompletionSource = new TaskCompletionSource<TaskResult>();

            if (_pipeClient.IsConnected)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                _pipeClient.BeginWrite(buffer, 0, buffer.Length, asyncResult =>
                {
                    try
                    {
                        taskCompletionSource.SetResult(EndWriteCallBack(asyncResult));
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetException(ex);
                    }
                }, null);
            }
            else
            {
                Logger.Error("Cannot send message, pipe is not connected");
                throw new IOException("pipe is not connected");
            }

            return taskCompletionSource.Task;
        }

        #endregion ICommunicationClient implementation

        #region private methods

        /// <summary>
        /// This callback is called when the BeginWrite operation is completed.
        /// It can be called whether the connection is valid or not.
        /// </summary>
        /// <param name="asyncResult"></param>
        private TaskResult EndWriteCallBack(IAsyncResult asyncResult)
        {
            _pipeClient.EndWrite(asyncResult);
            _pipeClient.Flush();

            return new TaskResult { IsSuccess = true };
        }

        #endregion private methods
    }
}