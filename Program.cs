using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using FTPClient.Services;

namespace FTPClient
{
    internal static class Program
    {
        private static FtpService ftp = null;

        private static void Main(string[] args)
        {
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
                                if (cmd.Split(" ").Length > 1)
                                {
                                    ftp.ReceiveData("LIST " + cmd.Split(" ")[1]).Print();
                                }

                                ftp.ReceiveData("LIST").Print();
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
                                byte[] data = ftp.ReceiveFile(cmd.Split(" ")[1]);
                                File.WriteAllBytes(cmd.Split(" ")[2], data);
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

                                byte[] data = ftp.ReceiveFile(fromPath);
                                File.WriteAllBytes(toPath, data);
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
                        case "rrmdir":
                            DeleteFolderRecursively(cmd.Split(" ")[1]);
                            break;
                        case "zip": 
                            ConstructZipFile(cmd.Split(" ")[1], cmd.Split(" ")[2]);
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

        private static void DeleteFolderRecursively(string folderName)
        {
            FTPFolderStructure folderStructure = null;

            ftp.ExecuteCommand("CWD " + folderName);
            folderStructure = new FTPFolderStructure(ftp.ReceiveData("LIST"));

            foreach(var file in folderStructure.GetFiles())
            {
                ftp.ExecuteCommand("DELE " + file);
            }

            foreach(var directory in folderStructure.GetDirectories())
            {
                DeleteFolderRecursively(directory);
            }

            ftp.ExecuteCommand("CWD ..");
            ftp.ExecuteCommand("RMD " + folderName);
        }

        private static void ConstructZipFile(string folderName, string savePath)
        {
            using FileStream zip = new FileStream(savePath, FileMode.OpenOrCreate);
            using ZipArchive archive = new ZipArchive(zip, ZipArchiveMode.Update);

            List<string> filePaths = new();
            GetAllFilePaths(folderName, ref filePaths);
            foreach(var file in filePaths)
            {
                ZipArchiveEntry newEntry = archive.CreateEntry(file);
                BinaryWriter bw = new BinaryWriter(newEntry.Open());

                byte[] data = ftp.ReceiveFile(file);
                bw.Write(data);
            }
        }

        private static void GetAllFilePaths(string folderPath, ref List<string> filePaths)
        {
            var folderStructure = new FTPFolderStructure(
                ftp.ReceiveData("LIST " + folderPath));

            foreach(var filePath in folderStructure.GetFiles())
            {
                filePaths.Add(folderPath + "/" + filePath);
            }

            foreach(var directory in folderStructure.GetDirectories())
            {
                GetAllFilePaths(folderPath + "/" + directory, ref filePaths);
            }
        }
    }
}
