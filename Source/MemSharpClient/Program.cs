using MemSharpClient.Network;
using System;
using System.Diagnostics;

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

                if(!Debugger.IsAttached)
                    MemSharpPacketFactory.SendWorkingDirectory(client, @"E:\Other\Repositories\IgniteFun\IgniteFun\bin\Debug\netstandard2.0");
                else
                    MemSharpPacketFactory.SendWorkingDirectory(client, @"E:\Other\Repositories\MemSharp\Samples\MemSharpSampleScript\bin\Debug\netstandard2.0");
                MemSharpPacketFactory.SendStartScriptDomain(client);

                Console.WriteLine("ScriptDomain is running...");
               
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