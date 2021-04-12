using System;
using ConsoleFTPClient.Services;
using FTPClient.Services;

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
                        Console.Write(ftp.ExecuteCommand("LIST").Message);
                        Console.Write(ftp.ReceiveData().Message);
                        break;
                    case "cd":
                        Console.Write(ftp.ExecuteCommand($"CWD {cmd.Split(" ")[1]}").Message);
                        break;
                    case "get":
                        Console.WriteLine(ftp.ExecuteCommand($"RETR {cmd.Split(" ")[1]}").Message);
                        FileBuilder fb = new FileBuilder(ftp.ReceiveData().ByteCode);
                        fb.SaveFile(@$"C:\Users\povilas\Downloads\{cmd.Split(" ")[1]}");
                        break;
                    default:
                        System.Console.WriteLine("Command not found!");
                        break;
                }
            }

        }
    }
}
