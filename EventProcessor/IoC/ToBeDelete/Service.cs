using System;
using System.Diagnostics;

namespace EventProcessor.IoC.ToBeDelete
{
    public class Service : IService
    {
        private readonly IExternal external;

        public Service(IExternal external)
        {
            this.external = external;
        }

        public void Do()
        {
            Debug.WriteLine("do something . . .");
            external.Extern();
        }
    }
}
