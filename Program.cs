using System;
using System.Net;
using System.Net.Sockets;

namespace FTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketService sv = new SocketService("test.rebex.net", 21);

            if (!sv.Connect())
            {
                Environment.Exit(0);
            }

            Console.WriteLine(sv.Receive());

            while(true)
            {
                Console.Write("ftp> ");
                string command = Console.ReadLine();
                sv.Send(command);
                Console.Write(sv.Receive());
            }
        }
    }
}
