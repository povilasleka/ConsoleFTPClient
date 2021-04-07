using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTPClient
{
    public class SocketService
    {
        private IPEndPoint endPoint;
        private Socket socket;

        public SocketService(string address, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
            IPAddress connAddr = ipHostInfo.AddressList[0];
            endPoint = new IPEndPoint(connAddr, port);
        }

        public SocketService(IPAddress address, int port)
        {
            endPoint = new IPEndPoint(address, port);
        }

        public bool Connect()
        {
            try 
            {
                Socket client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(endPoint);
                this.socket = client;
                
                return true;
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("Connection failed: " + e);
            }

            return false;
        }

        public string Receive()
        {
            byte[] response = new byte[100];
            socket.Receive(response);
            return Encoding.ASCII.GetString(response, 0, response.Length);
        }

        public void Send(string message)
        {
            byte[] messageInBytes = Encoding.ASCII.GetBytes(message + "\r\n");
            socket.Send(messageInBytes);
        }
    }
}