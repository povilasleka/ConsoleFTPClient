using System;
using System.IO;

namespace FTPClient 
{
    public class FTPFile : IDisposable
    {
        private string _path;
        private FileStream _fs = null;
        private BinaryWriter _binaryWriter = null;
        private BinaryReader _binaryReader = null;
        private int _readPointerAt = 0;

        public FTPFile(string path, string mode)
        {
            _path = path;

            if (mode == "r")
            {
                CreateFileStreamForReading();
            }
            else if (mode == "w")
            {
                //CreateFileStreamForWriting();
            }
            else
            {
                throw new ArgumentException("FTPFile mode is not correct.");
            }
        }

        public long Length { get => _fs.Length; }

        public void Dispose()
        {
            //_fs?.Dispose();
            //_binaryWriter?.Dispose();
            //_binaryReader?.Dispose();
        }

        public byte[] Read(int numBytes)
        {
            byte[] data = new byte[numBytes];

            _binaryReader.Read(data, _readPointerAt, numBytes);
            _readPointerAt += numBytes;

            return data;
        }

        public void Write(byte[] data)
        {
            // temporary solution.
            File.WriteAllBytes(_path, data);
        }

        private void CreateFileStreamForReading()
        {
            if (_fs != null)
                return;

            if (!File.Exists(_path))
            {
                throw new FileNotFoundException("File with given path does not exist.");
            }
            else
            {
                _fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                _binaryReader = new BinaryReader(_fs);
            }
        }

        private void CreateFileStreamForWriting()
        {
            if (_fs != null)
                return;

            if (!File.Exists(_path))
            {
                _fs = File.Create(_path);
                _binaryWriter = new BinaryWriter(_fs);
            }
            else
            {
                _fs = new FileStream(_path, FileMode.Append, FileAccess.ReadWrite, FileShare.None);
                _binaryWriter = new BinaryWriter(_fs);
            }
        }
    }
}