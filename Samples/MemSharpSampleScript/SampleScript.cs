using MemSharpCommon;
using System;
using System.Diagnostics;
using System.Text;

namespace MemSharpSampleScript
{
    public class SampleScript : Script
    {
        private readonly IntPtr ProcessBaseAddress;

        public SampleScript()
        {
            ProcessBaseAddress = Process.GetCurrentProcess().MainModule.BaseAddress;
        }

        public override void Initialize()
        {
            Console.Clear();
            Console.WriteLine("ProcessBaseAddress: 0x{0:X8}", ProcessBaseAddress.ToInt32());

            unsafe
            {
                var header = *(ushort*)(ProcessBaseAddress);
                Console.WriteLine(Encoding.Default.GetString(BitConverter.GetBytes(header)));
            }
        }
    }
}