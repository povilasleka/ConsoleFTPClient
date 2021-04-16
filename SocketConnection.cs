using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTPClient
{
    public class SocketConnection : IDisposable
    {
        private readonly IPEndPoint _endPoint;
        private Socket _socket;

        public SocketConnection(string address, int port)
        {
            var ipHostInfo = Dns.GetHostEntry(address);
            var connAddr = ipHostInfo.AddressList[0];
            _endPoint = new IPEndPoint(connAddr, port);
        }

        public SocketConnection(IPAddress address, int port)
        {
            _endPoint = new IPEndPoint(address, port);
        }

        public void Connect()
        {
            try 
            {
                _socket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_endPoint);
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("Connection failed: " + e);
            }
        }

        public void Dispose()
        {
            _socket?.Close();
        }

        public SocketResponse Receive()
        {
            var response = new byte[100];
            if (_socket.Receive(response) == 0)
                return default;

            var socketResponse = new SocketResponse(response);
            return socketResponse;
        }

        public void Send(string message)
        {
            var messageInBytes = Encoding.ASCII.GetBytes(message + "\r\n");
            _socket.Send(messageInBytes);
        }
    }
}