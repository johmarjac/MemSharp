using System;

namespace SampleServer
{
    internal class Program
    {
        private static void Main()
        {
            Console.Title = "Ether.Network Server";

            using (var server = new SampleServer())
                server.Start();
        }
    }
}