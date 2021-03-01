using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Diagnostics;
using System.IO.Pipes;
using System.Threading;

namespace IPC.NamedPipe
{
    public delegate void DelegateMessage(PipeMessage Reply);

    public class Server
    {
        public event DelegateMessage ReceivedCallback;
        string _pipeName;
        NamedPipeServerStream pipeServer;

        public void Listen(string PipeName)
        {
            try
            {
                _pipeName = PipeName;
                pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
               pipeServer.BeginWaitForConnection(new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch (Exception oEX)
            {
                Debug.WriteLine(oEX.Message);
            }
        }

        private void WaitForConnectionCallBack(IAsyncResult iar)
        {
            try
            {
                pipeServer = (NamedPipeServerStream)iar.AsyncState;
                pipeServer.EndWaitForConnection(iar);

                byte[] buffer = new byte[6];
                pipeServer.Read(buffer, 0, 6);
                byte type = buffer[2];
                if (buffer[3] == (byte)(buffer[2] + ((buffer[0] ^ buffer[1]))))
                {
                    PipeMessage message = new PipeMessage();

                    int size = (buffer[0] << 8 | buffer[1]);
                    buffer = new byte[size];
                    pipeServer.Read(buffer, 0, size);
                    message.SetPayload(typeof(byte[]), buffer);

                    if (type == 1)
                        message.ConvertToString();
                    if (type == 2)
                        message.ConvertToBytes();
                    ReceivedCallback.Invoke(message);
                }
               
                pipeServer.Close();
                pipeServer = null;
                pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut,
                   1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                pipeServer.BeginWaitForConnection(
                   new AsyncCallback(WaitForConnectionCallBack), pipeServer);
            }
            catch
            {
                return;
            }
        }

    }
}
