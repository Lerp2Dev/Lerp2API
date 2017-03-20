using Lerp2API.DebugHandler;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lerp2API.Communication.Sockets
{
    public class SocketServer
    {
        /*static void Main(string[] args)
        {
            // Creates one SocketPermission object for access restrictions
            SocketPermission permission = new SocketPermission(
                NetworkAccess.Accept,     // Allowed to accept connections
                TransportType.Tcp,        // Defines transport types
                "",                       // The IP addresses of local host
                SocketPermission.AllPorts // Specifies all ports
                );

            // Listening Socket object
            Socket sListener = null;

            try
            {
                // Ensures the code to have permission to access a Socket
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 4510);

                // Create one Socket object to listen the incoming connection
                sListener = new Socket(
                    ipAddr.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp
                    );

                // Associates a Socket with a local endpoint
                sListener.Bind(ipEndPoint);

                // Places a Socket in a listening state and specifies the maximum
                // Length of the pending connections queue
                sListener.Listen(10);

                UnityEngine.Debug.Log("Waiting for a connection on port {0}",
                    ipEndPoint);

                // Begins an asynchronous operation to accept an attempt
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                sListener.BeginAccept(aCallback, sListener);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("Exception: {0}", ex.ToString());
                return;
            }

            UnityEngine.Debug.Log("Press the Enter key to exit ...");
            Console.ReadLine();

            if (sListener.Connected)
            {
                sListener.Shutdown(SocketShutdown.Receive);
                sListener.Close();
            }
        }*/

        public const int lerpedPort = 22222;

        public Socket ServerSocket;
        public SocketPermission Permision;
        public IPAddress IP;
        public int Port;
        private IPEndPoint _endpoint;
        private byte[] bytes;

        private static bool debug;

        private AsyncCallback _callback;
        public AsyncCallback ServerCallback
        {
            set
            {
                _callback = value;
                AssignCallback(_callback);
            }
        }

        internal IPEndPoint IPEnd
        {
            get
            {
                if (IP != null)
                {
                    if (_endpoint == null)
                        _endpoint = new IPEndPoint(IP, Port);
                    return _endpoint;
                }
                else return null;
            }
        }

        public SocketServer(bool debug, bool doConnection = false) : 
            this(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), Dns.GetHostEntry("").AddressList[0], lerpedPort, SocketType.Stream, ProtocolType.Tcp, debug, doConnection)
        { }

        public SocketServer(string ip, int port, bool debug, bool doConnection = false) : 
            this(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), IPAddress.Parse(ip), port, SocketType.Stream, ProtocolType.Tcp, debug, doConnection)
        { }

        public SocketServer(SocketPermission permission, IPAddress ipAddr, int port, SocketType sType, ProtocolType pType, bool curDebug, bool doConnection = false)
        {
            permission.Demand();

            IP = ipAddr;
            Port = port;

            debug = curDebug;

            ServerSocket = new Socket(ipAddr.AddressFamily, sType, pType);

            bytes = new byte[1024];

            if (doConnection) ServerSocket.Bind(IPEnd);
        }

        public void ComeAlive()
        {
            IPEndPoint end = IPEnd;
            if (end != null) ServerSocket.Bind(end);
            else Debug.LogError("Destination IP isn't defined!");
        }

        public void StartListening()
        {
            if (ServerSocket.IsBound) ServerSocket.Listen(10);
            else Debug.LogError("You have to make alive your Server socket first! (Call 'ComeAlive' method)");
        }

        private void AssignCallback(AsyncCallback aCallback)
        {
            if(aCallback == null)
            {
                Debug.LogError("Server callback cannot be null");
                return;
            }
            //aCallback = new AsyncCallback(AcceptCallback);
            ServerSocket.BeginAccept(aCallback, ServerSocket);
        }

        public void CloseServer()
        {
            if (ServerSocket.Connected)
            {
                ServerSocket.Shutdown(SocketShutdown.Receive);
                ServerSocket.Close();
            }
            else Debug.LogError("If you want to close something, you have to be first connected!");
        }

        /// <summary>
        /// Asynchronously accepts an incoming connection attempt and creates
        /// a new Socket to handle remote host communication.
        /// </summary>     
        /// <param name="ar">the status of an asynchronous operation
        /// </param> 
        public static void AcceptCallback(IAsyncResult ar)
        {
            Socket listener = null;

            // A new Socket to handle remote host communication
            Socket handler = null;
            try
            {
                // Receiving byte array
                byte[] buffer = new byte[1024];
                // Get Listening Socket object
                listener = (Socket)ar.AsyncState;
                // Create a new socket
                handler = listener.EndAccept(ar);

                // Using the Nagle algorithm
                handler.NoDelay = false;

                // Creates one object array for passing data
                object[] obj = new object[2];
                obj[0] = buffer;
                obj[1] = handler;

                // Begins to asynchronously receive data
                handler.BeginReceive(
                    buffer,        // An array of type Byt for received data
                    0,             // The zero-based position in the buffer 
                    buffer.Length, // The number of bytes to receive
                    SocketFlags.None,// Specifies send and receive behaviors
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate
                    obj            // Specifies infomation for receive operation
                    );

                // Begins an asynchronous operation to accept an attempt
                AsyncCallback aCallback = new AsyncCallback(AcceptCallback);
                listener.BeginAccept(aCallback, listener);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Asynchronously receive data from a connected Socket.
        /// </summary>
        /// <param name="ar">
        /// the status of an asynchronous operation
        /// </param> 
        public static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Fetch a user-defined object that contains information
                object[] obj = new object[2];
                obj = (object[])ar.AsyncState;

                // Received byte array
                byte[] buffer = (byte[])obj[0];

                // A Socket to handle remote host communication.
                Socket handler = (Socket)obj[1];

                // Received message
                string content = string.Empty;

                // The number of bytes received.
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    content += Encoding.Unicode.GetString(buffer, 0,
                        bytesRead);
                    // If message contains "<Client Quit>", finish receiving
                    if (content.IndexOf("<Client Quit>") > -1)
                    {
                        // Convert byte array to string
                        string str =
                            content.Substring(0, content.LastIndexOf("<Client Quit>"));
                        if(debug)
                            UnityEngine.Debug.LogFormat(
                                "Read {0} bytes from client.\n Data: {1}",
                                str.Length * 2, str);

                        // Prepare the reply message
                        byte[] byteData =
                            Encoding.Unicode.GetBytes(str);

                        // Sends data asynchronously to a connected Socket
                        handler.BeginSend(byteData, 0, byteData.Length, 0,
                            new AsyncCallback(SendCallback), handler);
                    }
                    else
                    {
                        // Continues to asynchronously receive data
                        byte[] buffernew = new byte[1024];
                        obj[0] = buffernew;
                        obj[1] = handler;
                        handler.BeginReceive(buffernew, 0, buffernew.Length,
                            SocketFlags.None,
                            new AsyncCallback(ReceiveCallback), obj);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("Exception: {0}", ex.ToString());
            }
        }

        /// <summary>
        /// Sends data asynchronously to a connected Socket.
        /// </summary>
        /// <param name="ar">
        /// The status of an asynchronous operation
        /// </param> 
        public static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // A Socket which has sent the data to remote host
                Socket handler = (Socket)ar.AsyncState;

                // The number of bytes sent to the Socket
                int bytesSend = handler.EndSend(ar);
                if (debug)
                    UnityEngine.Debug.LogFormat(
                        "Sent {0} bytes to Client", bytesSend);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogFormat("Exception: {0}", ex.ToString());
            }
        }
    }
}