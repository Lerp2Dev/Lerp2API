using Lerp2API.Communication.Sockets;
using System;

namespace Lerp2API.DynFixes
{
    public class DynSocketServer : SocketServer
    {
        public SocketServer baseIns;
        
        public DynSocketServer()
        {
            baseIns = new SocketServer();
        }

        public void SetServerCallback(AsyncCallback serverCallback)
        {
            ServerCallback = serverCallback;
        }
    }
}
