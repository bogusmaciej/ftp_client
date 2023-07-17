using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ftp_klient
{
    public class MessageEventArgs : EventArgs
    {
        public string message;
    }

    public class Client
    {
        private string ip;
        Socket sender;

        public delegate void MessageRecievedEventHandler(object source, MessageEventArgs args);
        public event MessageRecievedEventHandler MessageRecieved;

        public Client(string ip_)
        {
            ip = ip_;
        }

        public async Task startClient()
        {
            IPAddress ipAddr = IPAddress.Parse(ip);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
            sender = new Socket(ipAddr.AddressFamily,SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sender.Connect(localEndPoint);
                sender.Send(Encoding.ASCII.GetBytes("connected"));
                Console.WriteLine($"Socket connected to {sender.RemoteEndPoint.ToString()}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unexpected exception : {e}");
            }
        }

        public async Task receiveMessages()
        {
            byte[] messageReceived = new byte[1024];
            while (true)
            {
                if (sender.Connected)
                {
                    int byteRecv = await Task.Run(() => sender.Receive(messageReceived));
                    OnMessageRecieved(Encoding.ASCII.GetString(messageReceived, 0, byteRecv));
                }
                else
                {
                    break;
                }
            }
        }

        public void sendMessage(string msg)
        {
            sender.Send(Encoding.ASCII.GetBytes(msg));
        }

        public void shutDownClient()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Disconnect(true);
        }

        public string getSeverIp()
        {
            return ((IPEndPoint)(sender.RemoteEndPoint)).Address.ToString();
        }

        public bool IsConnected()
        {
            if (sender is null) return false;
            else if (sender.Connected) return true;
            else return false;
        }

        protected virtual void OnMessageRecieved(string message_)
        {
            if (MessageRecieved != null)
            {
                MessageRecieved(this, new MessageEventArgs() { message = message_ });
            }
        }
    }
}
