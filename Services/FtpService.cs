using System;
using System.IO;
using System.Linq;
using System.Net;
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
            _controlConnection.Receive(100);

            _controlConnection.Send($"USER {user}");
            var _ = _controlConnection.Receive(100);

            _controlConnection.Send($"PASS {pass}");
            return _controlConnection.Receive(100).ResponseCode == 230;
        }

        public SocketResponse ExecuteCommand(string command)
        {
            _controlConnection.Send(command);
            var response = _controlConnection.Receive(100);

            return response;
        }

        public void Upload(string sourcePath, string destPath)
        {
            OpenDataConnection();
            ExecuteCommand($"STOR {destPath}").Print();

            var bytes = File.ReadAllBytes(sourcePath);
            _dataConnection.Send(bytes);
            _dataConnection.Dispose();
            _controlConnection.Receive(50).Print();
        }

        public SocketResponse ReceiveData(string command = "")
        {
            OpenDataConnection();

            if (command != string.Empty)
                ExecuteCommand(command).Print();

            var connectResponse = _controlConnection.Receive(100);
            connectResponse.Print();

            using var ms = new MemoryStream();
            while (true)
            {
                var localResponse = _dataConnection.Receive(100);
                if (localResponse == default)
                    break;
                    
                ms.Write(localResponse.ByteCode);
            }

            return new SocketResponse(ms.ToArray());
        }

        public byte[] ReceiveFile(string filePath, int offset = 0)
        {
            string executeResult = ExecuteCommand($"SIZE {filePath}").Message;
            int fileSize = int.Parse(executeResult.Split(" ")[1]);

            using MemoryStream ms = new MemoryStream();

            OpenDataConnection();
            ExecuteCommand($"RETR {filePath}").Print();

            _controlConnection.Receive(100).Print();

            while(fileSize > 0)
            {
                int chunkSize = 8;
                if (fileSize < chunkSize)
                {
                    chunkSize = fileSize;
                }

                fileSize -= chunkSize;

                var localResponse = _dataConnection.Receive(chunkSize);
                ms.Write(localResponse.ByteCode);
            }

            _dataConnection.Dispose();

            return ms.ToArray();
        }
        
        private void OpenDataConnection()
        {
            _dataConnection?.Dispose();

            _controlConnection.Send("PASV");
            var recv = _controlConnection.Receive(100).Message;

            var (ip, port) = CalculatePasvIpAddressFromResponse(recv);

            _dataConnection = new SocketConnection(IPAddress.Parse(ip), port);
            _dataConnection.Connect();
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