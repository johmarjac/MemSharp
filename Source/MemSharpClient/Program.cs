using MemSharpClient.Network;
using System;
using System.Diagnostics;
using System.IO;

namespace MemSharpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Network.MemSharpClient())
            {
                client.Connect();
                if (!client.IsConnected)
                    return;

                if(args.Length == 1 && Directory.Exists(args[1]))
                {
                    MemSharpPacketFactory.SendWorkingDirectory(client, args[1]);
                }
                else
                {
                    Console.WriteLine("Enter Script/Assembly Path: ");
                    var inp = Console.ReadLine();
                    if (Directory.Exists(inp))
                        MemSharpPacketFactory.SendWorkingDirectory(client, inp);
                    else
                    {
                        Console.WriteLine("Directory does not exist.");
                        return;
                    }
                }
                
                Console.WriteLine("Available commands: start, stop, workingdir <path>, restart, shutdown");
                while(true)
                {
                    bool bExitLoop = false;
                    var input = Console.ReadLine();
                    switch(input)
                    {
                        case "start":
                            Console.WriteLine("Starting ScriptDomain...");
                            MemSharpPacketFactory.SendStartScriptDomain(client);
                            break;
                        case "stop":
                            Console.WriteLine("Stopping ScriptDomain...");
                            MemSharpPacketFactory.SendStopScriptDomain(client);
                            break;
                        case "workingdir":
                            Console.Write("Enter path to working directory: ");
                            MemSharpPacketFactory.SendWorkingDirectory(client, Console.ReadLine());
                            Console.WriteLine("Server has been restarted...");
                            break;
                        case "restart":
                            Console.WriteLine("Restarting ScriptDomain...");
                            MemSharpPacketFactory.SendStopScriptDomain(client);
                            MemSharpPacketFactory.SendStartScriptDomain(client);
                            break;
                        case "shutdown":
                            Console.WriteLine("Shutting down Server...");
                            MemSharpPacketFactory.SendShutdownServer(client);
                            bExitLoop = true;
                            break;
                    }

                    if (bExitLoop)
                        break;
                }

                client.Disconnect();
            }
        }
    }
}