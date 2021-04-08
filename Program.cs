using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ConsoleFTPClient.Services;

namespace ConsoleFTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FTPService ftp = new FTPService("195.144.107.198", 21);

            Console.Write("Username: ");
            string user = Console.ReadLine();
            Console.Write("Password: ");
            string pass = Console.ReadLine();

            if (ftp.Login(user, pass))
            {
                Console.WriteLine("Logged in!");
            }
            else
            {
                Console.WriteLine("Login failed!");
                Environment.Exit(-1);
            }

            while(true)
            {
                Console.Write("ftp> ");
                Console.WriteLine(ftp.ExecuteCommand(Console.ReadLine()));
            }

        }
    }
}
