using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleFTPClient.Services
{
    public class SocketConnection
    {
        private IPEndPoint endPoint;
        private Socket socket;

        public SocketConnection(string address, int port)
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(address);
            IPAddress connAddr = ipHostInfo.AddressList[0];
            endPoint = new IPEndPoint(connAddr, port);
        }

        public SocketConnection(IPAddress address, int port)
        {
            endPoint = new IPEndPoint(address, port);
        }

        public bool Connect()
        {
            try 
            {
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);

                return true;
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("Connection failed: " + e);
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Disconnect(false);
            socket.Close();
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