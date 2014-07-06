using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetExp
{
    /// <summary>
    /// 
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            programEntry progEntry = new programEntry(args);
            progEntry.Run();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    class programEntry
    {
        private string [] args;

        public programEntry(string[] argsVal)
        {
            args = argsVal;
        }

        public bool Run()
        {
            //new MediaFiles().Run();
            //new Performance().Run();
            //new structDictTestObj().Run();
            //new WMIClass().Run();
            //new TCPIPRegistry284280Object().Run(); 
            //new DeviceAPItest().Run();
            //new XMLTests().Run();
            //new TestThread().Run();
            //new CheckServiceJobObject().Run();
            //new RandomTest().Run();
            //new StringTest().Run();
            //new EvtEntries_265260().Run();
            //new ResgistryTest().Run();
            //new Devices().Run();
            //new RegistryTest().Run();
            new Web().Run(args);

            return true;
        }
    }
}
