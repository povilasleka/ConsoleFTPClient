using System.Collections.Generic;
using System.Linq;
using FTPClient.Services;

namespace FTPClient
{
    public class FTPFolderStructure
    {
        private Dictionary<string, bool> _files = new Dictionary<string, bool>(); 

        public FTPFolderStructure(SocketResponse socketResponse)
        {
            ParseSocketResponse(socketResponse);
        }

        public List<string> GetDirectories() 
        {
            return _files
                .Where(f => f.Value == true)
                .Select(f => f.Key)
                .ToList();
        }

        public List<string> GetFiles()
        {
            return _files
                .Where(f => f.Value == false)
                .Select(f => f.Key)
                .ToList();
        }

        private void ParseSocketResponse(SocketResponse socketResponse)
        {
            string[] lines = socketResponse.Message.Split("\r\n");
            if (lines.Length == 0)
                return;

            foreach(var line in lines)
            {
                if (line.Length == 0)
                    break;
                
                if (line.Contains('\0'))
                    break;

                bool isDirectory = line[0] == 'd';
                string fileName = line.Split(" ").Last();

                _files.Add(fileName, isDirectory);
            }
        }

    }
}