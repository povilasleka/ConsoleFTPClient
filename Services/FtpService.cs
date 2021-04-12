using System;
using System.Linq;
using System.Net;
using System.Text;
using FTPClient;

namespace FTPClient.Services
{
    public class FtpService
    {
        private readonly SocketConnection _controlConnection;
        private SocketConnection _dataConnection = null;

        public FtpService(string ip, int port)
        {
            _controlConnection = new SocketConnection(IPAddress.Parse(ip), port);
        }

        public bool Login(string user, string pass)
        {
            _controlConnection.Connect();
            _controlConnection.Receive();

            _controlConnection.Send($"USER {user}");
            var _ = _controlConnection.Receive();

            _controlConnection.Send($"PASS {pass}");
            return _controlConnection.Receive().ResponseCode == 230;
        }

        public SocketResponse ExecuteCommand(string command)
        {
            CreateDataConnection();
            _dataConnection.Connect();

            _controlConnection.Send(command);
            var response = _controlConnection.Receive();

            return response;
        }

        public SocketResponse ReceiveData()
        {
            var connectResponse = _controlConnection.Receive();

            if (connectResponse.ResponseCode != 226) return default(SocketResponse);
            Console.Write(connectResponse.Message);
            
            SocketResponse localResponse = null;
            var full = "";

            do
            {
                localResponse = _dataConnection.Receive();
                full += localResponse.Message;
            }
            while (!localResponse.LastRecord);

            return new SocketResponse(Encoding.ASCII.GetBytes(full));

        }
        
        private void CreateDataConnection()
        {
            _dataConnection?.Dispose();

            _controlConnection.Send("PASV");
            var recv = _controlConnection.Receive().Message;

            var (ip, port) = CalculatePasvIpAddressFromResponse(recv);

            _dataConnection = new SocketConnection(IPAddress.Parse(ip), port);
        }

        private static (string, int) CalculatePasvIpAddressFromResponse(string response)
        {
            var parts = response.Split(" ")[4] // we get the (a,b,c,d,x,y) structure with a lot of junk
                .Replace("(", "").Replace(")", "").Replace(".", "") // clearing the junk
                .Replace("\r", "").Replace("\0", "")
                .Split(","); 

            var ip = parts[0] + "." + parts[1] + "." + parts[2] + "." + parts[3];

            // port is calculated from (n,n,n,n,x,y) | PORT = x*256 + y
            var port = int.Parse(parts[4]) * 256 + int.Parse(parts[5]);

            return (ip, port);
        }
    }
}