using System;
using System.Threading.Tasks;
using FTPClient.Services;

namespace FTPClient
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // 195.144.107.198
            FtpService ftp = null;

            while(true)
            {
                Console.Write("ftp> ");

                var cmd = Console.ReadLine();
                if (cmd != null)
                    switch (cmd.Split(" ")[0])
                    {
                        case "open":
                            if (ftp != null) break;
                            ftp = new FtpService(cmd.Split(" ")[1], 21);
                            Console.Write("User: ");
                            var user = Console.ReadLine();
                            Console.Write("Password: ");
                            var pass = Console.ReadLine();
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
                            if (ftp != null)
                            {
                                ftp.Download("LIST").Print();
                            }
                            break;
                        case "cd":
                            if (ftp != null) 
                                ftp.ExecuteCommand($"CWD {cmd.Split(" ")[1]}").Print();
                            break;
                        case "cdup":
                            if (ftp != null)
                                ftp.ExecuteCommand($"CDUP").Print();
                            break;
                        case "delete":
                            if (ftp != null)
                                ftp.ExecuteCommand($"DELE {cmd.Split(" ")[1]}").Print();
                            break;
                        case "get":
                            if (ftp != null)
                            {
                                ftp.Download(cmd.Split(" ")[1], cmd.Split(" ")[2]);
                            }
                            break;
                        case "send":
                            if (ftp != null)
                            {
                                byte[] fileData = FileBuilder.Read(cmd.Split(" ")[1]);
                                ftp.Upload(fileData, cmd.Split(" ")[2]);
                            }
                            break;
                        case "binary":
                            if (ftp != null) 
                                ftp.ExecuteCommand("TYPE I").Print();
                            break;
                        case "ascii":
                            if (ftp != null)
                                ftp.ExecuteCommand("TYPE A").Print();
                            break;
                        case "bye":
                            Console.WriteLine("Bye!");
                            Environment.Exit(0);
                            break;
                        default:
                            System.Console.WriteLine("Command not found!");
                            break;
                    }
            }
            
        }
    }
}
