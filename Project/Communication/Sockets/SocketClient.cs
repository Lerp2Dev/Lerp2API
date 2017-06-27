using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using JsonUtility = Lerp2API.SafeECalls.JsonUtility;
using Logger = Lerp2API.SafeECalls.Logger;

namespace Lerp2API.Communication.Sockets
{
    /// <summary>
    /// Class SocketMessage.
    /// </summary>
    public class SocketMessage
    {
        /// <summary>
        /// The identifier
        /// </summary>
        public int id;

        //Name??
        /// <summary>
        /// The MSG
        /// </summary>
        public string msg;

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketMessage"/> class.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <param name="m">The m.</param>
        public SocketMessage(int i, string m)
        {
            id = i;
            msg = m;
        }
    }

    /// <summary>
    /// Class SocketClient.
    /// </summary>
    public class SocketClient
    { //Hacer IDisposable?
        /// <summary>
        /// The client socket
        /// </summary>
        public Socket ClientSocket;

        /// <summary>
        /// The ip
        /// </summary>
        public IPAddress IP;

        /// <summary>
        /// The port
        /// </summary>
        public int Port, Id;

        private IPEndPoint _endpoint;
        private byte[] socketBuffer; //I will keep this static, but I think I will have problems
        private Timer task;
        private Action act;
        private int period = 1;
        private Logger logger;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(string fileLog, bool doConnection = false) :
            this(IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, 1, null, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="everyFunc">The every function.</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(Action everyFunc, string fileLog, bool doConnection = false) :
            this(IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, 1, everyFunc, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(string ip, int port, string fileLog, bool doConnection = false) :
            this(ip, port, -1, null, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="everyFunc">The every function.</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(string ip, int port, Action everyFunc, string fileLog, bool doConnection = false) :
            this(ip, port, 1, everyFunc, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="readEvery">The read every.</param>
        /// <param name="everyFunc">The every function.</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(string ip, int port, int readEvery, Action everyFunc, string fileLog, bool doConnection = false) :
            this(IPAddress.Parse(ip), port, SocketType.Stream, ProtocolType.Tcp, readEvery, everyFunc, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketClient"/> class.
        /// </summary>
        /// <param name="ipAddr">The ip addr.</param>
        /// <param name="port">The port.</param>
        /// <param name="sType">Type of the s.</param>
        /// <param name="pType">Type of the p.</param>
        /// <param name="readEvery">The read every.</param>
        /// <param name="everyFunc">The every function.</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketClient(IPAddress ipAddr, int port, SocketType sType, ProtocolType pType, int readEvery, Action everyFunc, string fileLog, bool doConnection = false)
        {
            socketBuffer = new byte[1024];

            period = readEvery;

            act = everyFunc;
            TimerCallback timerDelegate = new TimerCallback(Timering);

            if (everyFunc != null)
                task = new Timer(timerDelegate, null, 5, readEvery);

            IP = ipAddr;
            Port = port;

            ClientSocket = new Socket(ipAddr.AddressFamily, sType, pType);
            ClientSocket.NoDelay = false;

            Id = ClientSocket.GetHashCode();

            logger = new Logger(Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath)), fileLog));

            if (doConnection)
            {
                ClientSocket.Connect(IPEnd);
                //if (cbTimer != null)
                StartReceiving();
            }
        }

        /// <summary>
        /// Starts the receiving.
        /// </summary>
        public void StartReceiving()
        {
            if (task != null)
                task.Change(5, period);
        }

        /// <summary>
        /// Stops the receiving.
        /// </summary>
        public void StopReceiving()
        {
            if (task != null)
                task.Change(5, 0);
        }

        /// <summary>
        /// Does the connection.
        /// </summary>
        public void DoConnection()
        {
            IPEndPoint end = IPEnd;
            if (end != null)
            {
                ClientSocket.Connect(end);
                StartReceiving();
                ClientSocket.Send(Encoding.Unicode.GetBytes(JsonUtility.ToJson(new SocketMessage(Id, "<conn>"))));
            }
            else logger.LogError("Destination IP isn't defined!");
        }

        //Esto lo tengo que arreglar
        /// <summary>
        /// Writes the specified MSG.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>System.Int32.</returns>
        public int Write(string msg)
        {
            return SendMessage(msg, false);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>System.Int32.</returns>
        public int WriteLine(string msg)
        {
            return SendMessage(msg, true);
        }

        private int SendMessage(string msg, bool breakLine)
        {
            string message = JsonUtility.ToJson(new SocketMessage(Id, msg));
            int bytesSend = ClientSocket.Send(Encoding.Unicode.GetBytes(message));
            //if (breakLine) BreakLine(); //Voy a desactivar esto temporalmente
            return bytesSend;
        }

        private void BreakLine()
        {
            ClientSocket.Send(Encoding.Unicode.GetBytes("<stop>"));
        }

        /// <summary>
        /// Receives the message.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool ReceiveMessage(out string msg) //No entiendo porque este es sincrono, deberia ser asincrono... Copy & paste rulez!
        { //Esto solo devolverá falso cuando se cierre la conexión...
            try
            {
                byte[] bytes = new byte[1024];

                // Receives data from a bound Socket.
                int bytesRec = ClientSocket.Receive(bytes);

                // Converts byte array to string
                msg = Encoding.Unicode.GetString(bytes, 0, bytesRec);

                // Continues to read the data till data isn't available
                while (ClientSocket.Available > 0)
                {
                    bytesRec = ClientSocket.Receive(bytes);
                    msg += Encoding.Unicode.GetString(bytes, 0, bytesRec);
                }

                if (msg == "<close>")
                {
                    logger.Log("Closing connection...");
                    WriteLine("<client_closed>");
                    End();
                    return false;
                }

                return true;
            }
            catch
            { //Forced connection close...
                msg = ""; //Dead silence.
                WriteLine("<client_closed>");
                End();
                return false;
            }
        }

        /*private void SocketCallback(object obj)
        {
            ReceiveMessage((byte[])obj);
        }*/

        private void CloseConnection(SocketShutdown soShutdown)
        {
            if (soShutdown == SocketShutdown.Receive)
            {
                logger.LogWarning("Remember that you're in a Client, you, you can't only close Both connections or only your connection.");
                return;
            }
            ClientSocket.Disconnect(false);
            ClientSocket.Shutdown(soShutdown);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            ClientSocket.Close();
        }

        /// <summary>
        /// Ends this instance.
        /// </summary>
        public void End()
        {
            CloseConnection(SocketShutdown.Both);
            //Dispose();
        }

        private void Timering(object stateInfo)
        {
            act();
        }
    }
}