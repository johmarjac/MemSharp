using System;

namespace MemSharpCommon
{
    public abstract class Script
    {
        public Script()
        {
            AppDomain.CurrentDomain.DomainUnload += (object sender, EventArgs e) => Uninitialize();
        }

        public abstract void Initialize();
        public abstract void Uninitialize();
    }
}