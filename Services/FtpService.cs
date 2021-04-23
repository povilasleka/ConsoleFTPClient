using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public SocketResponse Download(string retrPath, string savePath)
        {
            ExecuteCommand("TYPE I").Print();
            DownloadData(retrPath, savePath);

            return default;
        }

        public SocketResponse Download(string cmd)
        {
            ExecuteCommand("TYPE A").Print();

            return ReceiveData(cmd);
        }
        
        public void Upload(byte[] data, string fileName)
        {
            OpenDataConnection();
            ExecuteCommand($"STOR {fileName}").Print();

            _dataConnection.Send(data);
        }

        private SocketResponse ReceiveData(string command = "")
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

        private void DownloadData(string fromPath, string toPath)
        {
            
            int leftToDownload = int.Parse(
                    ExecuteCommand($"SIZE {fromPath}").Message.Split(" ")[1]);

            OpenDataConnection();

            ExecuteCommand($"RETR {fromPath}").Print();

            while (leftToDownload > 0)
            {
                int chunkSize = 100;
                if (leftToDownload < 100)
                {
                    chunkSize = leftToDownload;
                }
                leftToDownload -= 100;

                var localResponse = _dataConnection.Receive(chunkSize);
                FileBuilder.Write(toPath, localResponse.ByteCode);
            }
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