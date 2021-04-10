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

        public string Message 
        {
            get => Encoding.ASCII.GetString(ByteCode);
        }

        public int ResponseCode 
        {
            get => int.Parse(Message.Split(new char[] { ' ', '-' })[0]);
        }

        public bool LastRecord { get => Message.Contains('\0'); }
    }
}