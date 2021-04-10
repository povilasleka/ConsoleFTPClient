using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using FTPClient;

namespace ConsoleFTPClient.Services
{
    public class SocketConnection : IDisposable
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

        public void Connect()
        {
            try 
            {
                socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("Connection failed: " + e);
            }
        }

        public void Dispose()
        {
            if (socket != null)
                socket.Close();
        }

        /*public string Receive()
        {
            byte[] response = new byte[100];
            socket.Receive(response);
            return Encoding.ASCII.GetString(response, 0, response.Length);
        }*/

        public SocketResponse Receive()
        {
            byte[] response = new byte[100];
            socket.Receive(response);

            return new SocketResponse(response);
        }

        public byte[] ReceiveBytes()
        {
            byte[] response = new byte[100];
            socket.Receive(response);
            return response;
        }

        public byte[] SendReceive(byte[] data)
        {
            byte[] response = new byte[10];

            socket.Send(data);
            socket.Receive(response);

            return response;
        }

        public void Send(string message)
        {
            byte[] messageInBytes = Encoding.ASCII.GetBytes(message + "\r\n");
            socket.Send(messageInBytes);
        }
    }
}