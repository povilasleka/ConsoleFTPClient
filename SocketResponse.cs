using System.Text;

namespace FTPClient
{
    public class SocketResponse
    {
        public SocketResponse(byte[] data)
        {
            ByteCode = data;
        }

        public byte[] ByteCode { get; }

        public string Message => 
            Encoding.ASCII.GetString(ByteCode);

        public int ResponseCode => 
            int.Parse(Message.Split(new char[] { ' ', '-' })[0]);

        public int Size => ByteCode.Length;
    }
}