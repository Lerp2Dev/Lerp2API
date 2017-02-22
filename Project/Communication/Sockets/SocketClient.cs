using Lerp2API.DebugHandler;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lerp2API.Communication.Sockets
{
    public class SocketClient
    {
        /*static void Main(string[] args)
        {
            // Receiving byte array 
            byte[] bytes = new byte[1024];
            try
            {
                // Create one SocketPermission for socket access restrictions
                SocketPermission permission = new SocketPermission(
                    NetworkAccess.Connect,    // Connection permission
                    TransportType.Tcp,        // Defines transport types
                    "",                       // Gets the IP addresses
                    SocketPermission.AllPorts // All ports
                    );

                // Ensures the code to have permission to access a Socket
                permission.Demand();

                // Resolves a host name to an IPHostEntry instance           
                IPHostEntry ipHost = Dns.GetHostEntry("");

                // Gets first IP address associated with a localhost
                IPAddress ipAddr = ipHost.AddressList[0];

                // Creates a network endpoint
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 22222);

                // Create one Socket object to setup Tcp connection
                Socket sender = new Socket(
                    ipAddr.AddressFamily,// Specifies the addressing scheme
                    SocketType.Stream,   // The type of socket 
                    ProtocolType.Tcp     // Specifies the protocols 
                    );

                sender.NoDelay = false;   // Using the Nagle algorithm

                // Establishes a connection to a remote host
                sender.Connect(ipEndPoint);
                Console.WriteLine("Socket connected to {0}",
                    sender.RemoteEndPoint.ToString());

                // Sending message
                //<Client Quit> is the sign for end of data
                string theMessage = "Hello World!";
                byte[] msg = Encoding.Unicode.GetBytes(theMessage + "<Client Quit>");

                // Sends data to a connected Socket.
                int bytesSend = sender.Send(msg);

                // Receives data from a bound Socket.
                int bytesRec = sender.Receive(bytes);

                // Converts byte array to string
                theMessage = Encoding.Unicode.GetString(bytes, 0, bytesRec);

                // Continues to read the data till data isn't available
                while (sender.Available > 0)
                {
                    bytesRec = sender.Receive(bytes);
                    theMessage += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                }
                Console.WriteLine("The server reply: {0}", theMessage);

                // Disables sends and receives on a Socket.
                sender.Shutdown(SocketShutdown.Both);

                //Closes the Socket connection and releases all resources
                sender.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.ToString());
            }

            Console.Read();
        }*/

        public Socket ClientSocket;
        public IPAddress IP;
        public int Port;
        private Timer thTimer;
        private IPEndPoint _endpoint;
        private byte[] socketBuffer; //I will keep this static, but I think I will have problems
        private int Period, DueTime;

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

        public SocketClient(bool doConnection = false) : 
            this(Dns.GetHostEntry("").AddressList[0], 22222, SocketType.Stream, ProtocolType.Tcp, null, null, 1, doConnection)
        { }

        public SocketClient(TimerCallback cbTimer, object obj, int readEvery, bool doConnection = false) :
            this(Dns.GetHostEntry("").AddressList[0], 22222, SocketType.Stream, ProtocolType.Tcp, cbTimer, obj, readEvery, doConnection)
        { }

        public SocketClient(string ip, int port, bool doConnection = false) : 
            this(IPAddress.Parse(ip), port, SocketType.Stream, ProtocolType.Tcp, null, null, 1, doConnection)
        { }

        public SocketClient(IPAddress ipAddr, int port, SocketType sType, ProtocolType pType, TimerCallback cbTimer, object obj, int readEvery, bool doConnection = false)
        {
            socketBuffer = new byte[1024];

            DueTime = 5;
            Period = readEvery;

            thTimer = new Timer(cbTimer != null ? cbTimer : SocketCallback, obj != null ? obj : socketBuffer, Timeout.Infinite, Timeout.Infinite);

            IP = ipAddr;
            Port = port;

            ClientSocket = new Socket(ipAddr.AddressFamily, sType, pType);
            ClientSocket.NoDelay = false;

            if (doConnection)
            {
                ClientSocket.Connect(IPEnd);
                //if (cbTimer != null) 
                StartReceiving();
            }
        }

        public void StartReceiving()
        {
            thTimer.Change(DueTime, Period);
        }

        public void StopReceiving()
        {
            thTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void DoConnection()
        {
            IPEndPoint end = IPEnd;
            if (end != null)
            {
                ClientSocket.Connect(end);
                StartReceiving();
            }
            else Debug.LogError("Destination IP isn't defined!");
        }

        public int Write(string msg)
        {
            return SendMessage(msg, false);
        }

        public int WriteLine(string msg)
        {
            return SendMessage(msg, true);
        }

        private int SendMessage(string msg, bool breakLine)
        {
            int bytesSend = ClientSocket.Send(Encoding.Unicode.GetBytes(msg));
            if (breakLine) BreakLine();
            return bytesSend;
        }

        private void BreakLine()
        {
            ClientSocket.Send(Encoding.Unicode.GetBytes("<Client Quit>"));
        }

        private string ReceiveMessage(byte[] bytes)
        {
            // Receives data from a bound Socket.
            int bytesRec = ClientSocket.Receive(bytes);

            // Converts byte array to string
            string msg = Encoding.Unicode.GetString(bytes, 0, bytesRec);

            // Continues to read the data till data isn't available
            while (ClientSocket.Available > 0)
            {
                bytesRec = ClientSocket.Receive(bytes);
                msg += Encoding.Unicode.GetString(bytes, 0, bytesRec);
            }
            return msg;
        }

        private void SocketCallback(object obj)
        {
            ReceiveMessage((byte[])obj);
        }

        public void CloseConnection(SocketShutdown soShutdown)
        {
            if(soShutdown == SocketShutdown.Receive)
            {
                Debug.LogError("Remember that you're in a Client, you, you can't only close Both connections or only your connection.");
                return;
            }
            ClientSocket.Shutdown(soShutdown);
        }

        public void DisposeSocket()
        {
            ClientSocket.Close();
        }

    }
}