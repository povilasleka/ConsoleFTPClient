using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FTPClient;

namespace ConsoleFTPClient.Services
{
    public class FTPService
    {
        private SocketConnection _controlConnection;
        private SocketConnection _dataConnection = null;

        public FTPService(string ip, int port)
        {
            _controlConnection = new SocketConnection(IPAddress.Parse(ip), port);
        }

        public bool Login(string user, string pass)
        {
            _controlConnection.Connect();
            _controlConnection.Receive();

            _controlConnection.Send($"USER {user}");
            _controlConnection.Receive();

            _controlConnection.Send($"PASS {pass}");
            if (_controlConnection.Receive().ResponseCode != 230)
                return false;

            return true;
        }

        public SocketResponse ExecuteCommand(string command)
        {
            CreateDataConnection();

            _controlConnection.Send(command + "\r\n");
            var response = _controlConnection.Receive();

            return response;
        }

        public SocketResponse ReceiveData()
        {
            _dataConnection.Connect();
            SocketResponse connectResponse = _controlConnection.Receive();

            if (connectResponse.ResponseCode == 226)
            {
                SocketResponse localResponse = null;
                string full = "";

                do
                {
                    localResponse = _dataConnection.Receive();
                    full += localResponse.Message;
                }
                while (!localResponse.LastRecord);

                return new SocketResponse(Encoding.ASCII.GetBytes(full));
            }

            return default(SocketResponse);
        }



        private void CreateDataConnection()
        {
            if (_dataConnection != null)
                _dataConnection.Dispose();

            _controlConnection.Send($"PASV");
            string recv = _controlConnection.Receive().Message;

            (string ip, int port) = CalculatePasvIpAddressFromResponse(recv);

            _dataConnection = new SocketConnection(IPAddress.Parse(ip), port);
        }

        public (string, int) CalculatePasvIpAddressFromResponse(string response)
        {
            var splitted = response.Split(" ")[4] // we get the (a,b,c,d,x,y) structure with a lot of junk
                .Replace("(", "").Replace(")", "").Replace(".", "") // clearing the junk
                .Replace("\r", "").Replace("\0", "")
                .Split(","); 

            string ip = splitted[0] + "." + splitted[1] + "." + splitted[2] + "." + splitted[3];

            // port is calculated from (n,n,n,n,x,y) | PORT = x*256 + y
            int port = int.Parse(splitted[4]) * 256 + int.Parse(splitted[5]);

            return (ip, port);
        }
    }
}