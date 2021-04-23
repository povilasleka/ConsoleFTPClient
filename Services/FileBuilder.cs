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
        }

        public static byte[] Read(string path)
        {
            byte[] data = new byte[] {};

            if (!File.Exists(path))
                return new byte[0];

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);

            var br = new BinaryReader(fs);
            br.Read(data, 0, (int) fs.Length);

            return data;
        }
    }
}
