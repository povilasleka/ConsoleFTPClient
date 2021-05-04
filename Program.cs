using System;
using System.IO;
using System.Linq;
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
                Console.Write("KT-ftp> ");

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
                            var pass = "";

                            while(true)
                            {
                                var key = Console.ReadKey(true);
                                if (key.Key == ConsoleKey.Enter)
                                    break;
                                
                                pass += key.KeyChar;
                            }
                            Console.WriteLine();

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
                        case "mdelete": 
                            for(int i = 1; i < cmd.Split(" ").Length; i++)
                            {
                                string path = cmd.Split(" ")[i];
                                ftp.ExecuteCommand($"DELE {cmd.Split(" ")[i]}").Print();
                            }
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
                                ftp.Upload(cmd.Split(" ")[1], cmd.Split(" ")[2]);
                            }
                            break;
                        case "mget":
							Console.Write("Directory: ");
							var directoryPath = Console.ReadLine();

							if (directoryPath.Last() != '/' || directoryPath.Last() != '\\')
							{
								directoryPath += '/';
							}

                            for(int i = 1; i < cmd.Split(" ").Length; i++)
                            {
                                string fromPath = cmd.Split(" ")[i];
                                string toPath = directoryPath + fromPath;

                                ftp.Download(fromPath, toPath);
                            }
                            break;
                        case "msend": 
                            for(int i = 1; i < cmd.Split(" ").Length; i++)
                            {
                                string fromPath = cmd.Split(" ")[i];
                                string toPath = fromPath.Split("/").Last();

                                ftp.Upload(fromPath, toPath);
                            }
                            break;
                        case "mkdir":
                            ftp.ExecuteCommand("MKD " + cmd.Split(" ")[1]).Print(); 
                            break;
                        case "rmdir":
                            ftp.ExecuteCommand("RMD " + cmd.Split(" ")[1]).Print();
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
                            ftp.ExecuteCommand("QUIT").Print();
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
