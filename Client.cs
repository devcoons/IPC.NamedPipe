using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;

namespace IPC.NamedPipe
{
    public class Client
    {
        NamedPipeClientStream pipeStream;
        
       
        public void Send(string rServer,string pipename,string SendStr)
        {
            try
            {
                try
                {

                    pipeStream = new NamedPipeClientStream(rServer, pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
                    pipeStream.Connect(1000);
                    Debug.WriteLine("[Client] Pipe connection established");
 
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Client] Pipe connection failed");
                    return ;
                }
                PipeMessage message = new PipeMessage();
                message.SetPayload(typeof(string), SendStr);
                byte[] toSend = message.GetSerialized();
                pipeStream.BeginWrite(toSend, 0, toSend.Length, new AsyncCallback(AsyncSend), pipeStream);
            }
            catch (TimeoutException oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        public void Send(string rServer, string pipename, byte[] SendBytes)
        {
            try
            {
                try
                {

                    pipeStream = new NamedPipeClientStream(rServer, pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
                    pipeStream.Connect(1000);
                    Debug.WriteLine("[Client] Pipe connection established");

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("[Client] Pipe connection failed");
                    return;
                }
                PipeMessage message = new PipeMessage();
                message.SetPayload(typeof(byte[]), SendBytes);
                byte[] toSend = message.GetSerialized();
                pipeStream.BeginWrite(toSend, 0, toSend.Length, new AsyncCallback(AsyncSend), pipeStream);
            }
            catch (TimeoutException oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState;

                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();

            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

    }
}
