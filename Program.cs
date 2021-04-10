using System;
using ConsoleFTPClient.Services;

namespace ConsoleFTPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // 195.144.107.198
            FTPService ftp = null;

            while(true)
            {
                Console.Write("ftp> ");

                string cmd = Console.ReadLine();
                switch(cmd.Split(" ")[0])
                {
                    case "open":
                        if (ftp != null) break;
                        ftp = new FTPService(cmd.Split(" ")[1], 21);
                        Console.Write("User: ");
                        string user = Console.ReadLine();
                        Console.Write("Password: ");
                        string pass = Console.ReadLine();
                        if (ftp.Login(user, pass))
                        {
                            Console.WriteLine($"Connected to {cmd.Split(" ")[1]}");
                        }
                        else
                        {
                            ftp = null;
                            Console.WriteLine("Connection failed!");
                        }
                        break;
                    case "ls":
                        ftp.ExecuteCommand("LIST");
                        Console.WriteLine(ftp.ReceiveData().Message);
                        break;
                    case "cd":
                        ftp.ExecuteCommand($"CWD {cmd.Split(" ")[1]}");
                        Console.WriteLine(ftp.ReceiveData().Message);
                        break;
                    default:
                        System.Console.WriteLine("Command not found!");
                        break;
                }
            }

        }
    }
}
