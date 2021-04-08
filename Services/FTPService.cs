using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleFTPClient.Services
{
    public class FTPService
    {
        private SocketConnection _controlConnection;
        private SocketConnection _dataConnection;

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

            if (GetResponseStatusCode(_controlConnection.Receive()) != 230)
                return false;

            return true;
        }

        public string ExecuteCommand(string command)
        {

            _controlConnection.Send(command);

            var response = _controlConnection.Receive();

            if (GetResponseStatusCode(response) != 150)
                return response;

            EstablishDataConnection();
            _dataConnection.Connect();
            if (GetResponseStatusCode(_controlConnection.Receive()) == 226)
            {
                string resp;
                string full = "";

                do
                {
                    resp = _dataConnection.Receive();
                    full += resp;
                }
                while (!resp.Contains('\0'));

                return full;
            }

            return "Command failed!";
        }

        private bool EstablishDataConnection()
        {
            _dataConnection = null;
            _controlConnection.Send($"PASV");
            string recv = _controlConnection.Receive();

            (string ip, int port) = CalculatePasvIpAddressFromResponse(recv);
            Console.WriteLine("Data Connection on " + ip + ":" + port);

            _dataConnection = new SocketConnection(IPAddress.Parse(ip), port);
            return true;
        }

        private int GetResponseStatusCode(string response)
        {
            return int.Parse(response.Split(new char[] { ' ', '-' })[0]);
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