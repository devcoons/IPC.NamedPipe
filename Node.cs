using System;
using System.Collections.Generic;
using System.Text;

namespace IPC.NamedPipe
{
    public delegate void ReceivedCallback(PipeMessage recvMessage);

    public class Node
    {
        private string lPipeName = ".";
        private string rPipeName = ".";
        private string rServer = ".";
        private Client client;
        private Server server;
        private ReceivedCallback callback = null;

        public Node(string localPipeName, string remotePipeName, string remoteHostname, ReceivedCallback callback)
        {
            lPipeName = localPipeName;
            rPipeName = remotePipeName;
            rServer = remoteHostname;
            this.callback = callback;
        }

        public bool Start()
        {
            try
            {
                server = new Server();
                server.ReceivedCallback += Server_PipeMessage;
                server.Listen(lPipeName + ".server");
                return true;
            }catch(Exception)
            {
                return false;
            }
        }
        
        public void Send(string arg)
        {
            client = new Client();
            client.Send(rServer,rPipeName+".server", arg);
        }

        public void Send(byte[] arg)
        {
            client = new Client();
            client.Send(rServer, rPipeName + ".server", arg);
        }


        private void Server_PipeMessage(PipeMessage Reply)
        {
            if(callback != null)
                callback.Invoke(Reply);  
        }
    }
}
