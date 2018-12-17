using MemSharpCommon;
using System;

namespace MemSharpSampleScript
{
    public class SampleScript : Script
    {
        public override void Initialize()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Hello from SampleScript!");
            Console.ResetColor();
        }
    }
}