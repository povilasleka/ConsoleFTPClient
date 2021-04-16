using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FTPClient.Services
{
    public static class FileBuilder
    {
        public static void Write(string path, byte[] data)
        {
            FileStream fs = null;
            if (!File.Exists(path))
            {
                fs = File.Create(path);
            }
            else
            {
                fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.None);
            }

            var bf = new BinaryWriter(fs);
            bf.Write(data);
            
            bf.Close();
            fs.Close();
            
            bf.Dispose();
            fs.Dispose();

            System.Console.WriteLine($"Written {data.Length} bytes.");
        }

        private static byte[] TrimZeroBytes(byte[] buffer)
        {
            while(true)
            {
                if (buffer[^1] == '\0')
                {
                    buffer = buffer.Take(buffer.Length - 1).ToArray();
                }
                else
                {
                    break;
                }
            }

            return buffer;
        }
    }
}
