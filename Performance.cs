using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;

namespace DotNetExp
{
    class Performance
    {
        public void Run()
        {
            // Create a RegistryKey, which will access the HKEY_PERFORMANCE_DATA
            // key in the registry of this machine.
            RegistryKey rk = Registry.PerformanceData;

            // Print out the keys.
            PrintKeys(rk);
            PrintKeys(rk);

            return;
        }

        private void PrintKeys(RegistryKey rkey)
        {

            // Retrieve all the subkeys for the specified key.
            String[] names = rkey.GetSubKeyNames();
            if (0 == names.Length)
                names = rkey.GetSubKeyNames();

            int icount = 0;

            Console.WriteLine("Subkeys of " + rkey.Name);
            Console.WriteLine("-----------------------------------------------");

            // Print the contents of the array to the console.
            foreach (String s in names)
            {
                Console.WriteLine(s);

                // The following code puts a limit on the number
                // of keys displayed.  Comment it out to print the
                // complete list.
                icount++;
                if (icount >= 10)
                    break;
            }
        }
    }
}
