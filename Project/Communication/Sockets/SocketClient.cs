using Lerp2API.SafeECalls;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Logger = Lerp2API.SafeECalls.Logger;
using JsonUtility = Lerp2API.SafeECalls.JsonUtility;
using System.IO;

namespace Lerp2API.Communication.Sockets
{
    public class SocketMessage
    {
        public int id;
        //Name??
        public string msg;

        public SocketMessage(int i, string m)
        {
            id = i;
            msg = m;
        }
    }

    public class SocketClient
    { //Hacer IDisposable?
        public Socket ClientSocket;
        public IPAddress IP;
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

        public SocketClient(bool doConnection = false) :
            this(IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, 1, null, doConnection)
        { }

        public SocketClient(Action everyFunc, bool doConnection = false) :
            this(IPAddress.Loopback, SocketServer.lerpedPort, SocketType.Stream, ProtocolType.Tcp, 1, everyFunc, doConnection)
        { }

        public SocketClient(string ip, int port, bool doConnection = false) :
            this(ip, port, -1, null, doConnection)
        { }

        public SocketClient(string ip, int port, Action everyFunc, bool doConnection = false) :
            this(ip, port, 1, everyFunc, doConnection)
        { }


        public SocketClient(string ip, int port, int readEvery, Action everyFunc, bool doConnection = false) :
            this(IPAddress.Parse(ip), port, SocketType.Stream, ProtocolType.Tcp, readEvery, everyFunc, doConnection)
        { }

        public SocketClient(IPAddress ipAddr, int port, SocketType sType, ProtocolType pType, int readEvery, Action everyFunc, bool doConnection = false)
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

            logger = new Logger(Path.Combine(Path.GetDirectoryName(Path.Combine(Application.dataPath, LerpedCore.defaultLogFilePath)), "client-Logger.log"));

            if (doConnection)
            {
                ClientSocket.Connect(IPEnd);
                //if (cbTimer != null) 
                StartReceiving();
            }
        }

        public void StartReceiving()
        {
            if (task != null)
                task.Change(5, period);
        }

        public void StopReceiving()
        {
            if (task != null)
                task.Change(5, 0);
        }

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
            string message = JsonUtility.ToJson(new SocketMessage(Id, msg));
            int bytesSend = ClientSocket.Send(Encoding.Unicode.GetBytes(message));
            //if (breakLine) BreakLine(); //Voy a desactivar esto temporalmente
            return bytesSend;
        }

        private void BreakLine()
        {
            ClientSocket.Send(Encoding.Unicode.GetBytes("<stop>"));
        }

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
            catch (Exception ex)
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

        public void Dispose()
        {
            ClientSocket.Close();
        }

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