using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FTPClient 
{
    public class File 
    {
        private string _path;
        private FileStream _fs = null;

        public File(string path)
        {
            _path = path;
        }

        public byte[] Read()
        {
            CreateFileStream(readOnly: true);

            // TODO: File read logic.
        }

        public void Write()
        {
            CreateFileStream(readOnly: false);

            // TODO: File write logic.
        }

        private void CreateFileStream(bool readOnly)
        {
            if (!System.IO.File.Exists())
            {
                if (!readOnly)
                    _fs = File.Create(_path);
                else
                    throw new FileNotFoundException("File with given path does not exist.");
            }
            else
            {
                _fs = new FileStream(_path);
            }
        }

        private void DestroyFileStream()
        {
            _fs.Dispose();
        }
    }
}