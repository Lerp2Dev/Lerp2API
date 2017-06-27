using Lerp2API.Hepers.JSON_Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// Class SocketServer.
    /// </summary>
    public class SocketServer
    {
        //The ClientInfo structure holds the required information about every
        //client connected to the server
        private struct ClientInfo
        {
            /// <summary>
            /// The socket
            /// </summary>
            public Socket socket;   //Socket of the client

            /// <summary>
            /// The string name
            /// </summary>
            public string strName;  //Name by which the user logged into the chat room
        }

        //The collection of all clients logged into the room (an array of type ClientInfo)
        private ArrayList clientList;

        /// <summary>
        /// The lerped port
        /// </summary>
        public const int lerpedPort = 22222;

        /// <summary>
        /// The server socket
        /// </summary>
        public Socket ServerSocket;

        /// <summary>
        /// The permision
        /// </summary>
        public SocketPermission Permision;

        /// <summary>
        /// The ip
        /// </summary>
        public IPAddress IP;

        /// <summary>
        /// The port
        /// </summary>
        public int Port;

        private IPEndPoint _endpoint;
        private byte[] byteData = new byte[1024];

        /// <summary>
        /// All done
        /// </summary>
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        /// <summary>
        /// The routing table
        /// </summary>
        public static Dictionary<int, Socket> routingTable = new Dictionary<int, Socket>();

        private static List<int> closedClients = new List<int>();

        private static bool debug;

        private AsyncCallback _callback;

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
        /// Initializes a new instance of the <see cref="SocketServer"/> class.
        /// </summary>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketServer(bool debug, string fileLog, bool doConnection = false) :
            this(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), Dns.GetHostEntry("").AddressList[0], lerpedPort, SocketType.Stream, ProtocolType.Tcp, debug, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="debug">if set to <c>true</c> [debug].</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketServer(string ip, int port, bool debug, string fileLog, bool doConnection = false) :
            this(new SocketPermission(NetworkAccess.Accept, TransportType.Tcp, "", SocketPermission.AllPorts), IPAddress.Parse(ip), port, SocketType.Stream, ProtocolType.Tcp, debug, fileLog, doConnection)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocketServer"/> class.
        /// </summary>
        /// <param name="permission">The permission.</param>
        /// <param name="ipAddr">The ip addr.</param>
        /// <param name="port">The port.</param>
        /// <param name="sType">Type of the s.</param>
        /// <param name="pType">Type of the p.</param>
        /// <param name="curDebug">if set to <c>true</c> [current debug].</param>
        /// <param name="fileLog">The file log.</param>
        /// <param name="doConnection">if set to <c>true</c> [do connection].</param>
        public SocketServer(SocketPermission permission, IPAddress ipAddr, int port, SocketType sType, ProtocolType pType, bool curDebug, string fileLog, bool doConnection = false)
        {
            permission.Demand();

            IP = ipAddr;
            Port = port;

            debug = curDebug;
            logger = new Logger(Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath)), "server-logger.log"));

            ServerSocket = new Socket(ipAddr.AddressFamily, sType, pType);

            if (doConnection) ServerSocket.Bind(IPEnd);
        }

        /// <summary>
        /// Comes the alive.
        /// </summary>
        public void ComeAlive()
        {
            if (IPEnd != null)
            {
                try
                {
                    ServerSocket.Bind(IPEnd);
                    ServerSocket.Listen(10); //El servidor se prepara para recebir la conexion de 10 clientes simultaneamente

                    logger.Log("Waiting for a connection...");
                    ServerSocket.BeginAccept(new AsyncCallback(OnAccept), ServerSocket);
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
            }
            else logger.LogError("Destination IP isn't defined!");
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = ServerSocket.EndAccept(ar);

                //Start listening for more clients
                ServerSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //Once the client connects then start receiving the commands from it
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), clientSocket);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    string str = Encoding.Unicode.GetString(byteData, 0, bytesRead); //Obtiene la longitud en bytes de los datos pasados y los transforma en una string
                    SocketMessage sm = null;
                    if (str.IsJson())
                        sm = JsonUtility.FromJson<SocketMessage>(str);

                    if (sm != null)
                    {
                        if (sm.msg == "<conn>")
                            routingTable.Add(sm.id, handler);
                        else if (sm.msg == "<close_clients>")
                        {
                            routingTable[sm.id].Send(Encoding.Unicode.GetBytes("<close>")); //First, close the client that
                            foreach (KeyValuePair<int, Socket> soc in routingTable)
                                if (soc.Key != sm.id) //Then, close the others one
                                    soc.Value.Send(Encoding.Unicode.GetBytes("<close>"));
                        }
                        else if (sm.msg == "<client_closed>")
                        {
                            closedClients.Add(sm.id);
                            if (closedClients.Count == routingTable.Count)
                                CloseServer(); //Close the server, when all the clients has been closed.
                        }
                        else
                        {
                            logger.Log("---------------------------");
                            logger.Log("Client with ID {0} sent {1} bytes (JSON).", sm.id, bytesRead);
                            logger.Log("Message: {0}", sm.msg);
                            logger.Log("Sending to the other clients.");
                            logger.Log("---------------------------");
                            logger.Log("");

                            //Send to the other clients
                            foreach (KeyValuePair<int, Socket> soc in routingTable)
                                if (soc.Key != sm.id)
                                    soc.Value.Send(Encoding.Unicode.GetBytes(str));
                        }
                    }
                    else
                    {
                        if (str == "<stop>") //Si recibe FINCONN sale
                            CloseServer();
                        //else if (str.IndexOf("<conn>") > -1)
                        //    routingTable.Add();
                        else
                            logger.LogError("Cannot de-encrypt the message!");
                    }
                }

                //Continua escuchando, para listar el próximo mensaje, recursividad asíncrona.
                handler.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), handler);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Called when [send].
        /// </summary>
        /// <param name="ar">The ar.</param>
        public void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        /// <summary>
        /// Closes the server.
        /// </summary>
        public void CloseServer()
        {
            if (ServerSocket.Connected)
            {
                ServerSocket.Shutdown(SocketShutdown.Receive);
                ServerSocket.Close();
            }
            else logger.LogError("If you want to close something, you have to be first connected!");
        }
    }
}