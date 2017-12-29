using System.Diagnostics;

namespace EventProcessor.IoC.ToBeDelete
{
    public class External : IExternal
    {
        public void Extern()
        {
            Debug.WriteLine("Extern something . . . ");
        }
    }
}
