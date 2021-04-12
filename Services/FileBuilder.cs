using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FTPClient.Services
{
    public class FileBuilder
    {
        private byte[] _buffer;

        public FileBuilder(byte[] buffer)
        {
            _buffer = buffer;
        }

        public void SaveFile(string path)
        {
            using (FileStream fs = File.Create(path))
            {
                fs.Write(_buffer);
            }
        }
        
    }
}
