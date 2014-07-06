using System;
using System.IO;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Collections;
using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace DotNetExp
{
    class TCPIPRegistry
    {
        private const string NetshCommandBase = "netsh interface ipv4 set address";
        public bool Run()
        {
            RegistryValues = new Dictionary<string, ArrayList>();
            RegistryValues.Clear();
            GetRegistryValues();
            return true;
        }

        private const string REGKEY_ROOT = @"SYSTEM\ControlSet001\services\Tcpip\Parameters\Interfaces";
        private const string REGKEY_VALUE_NAME = @"DefaultGateway";

        private Dictionary<String, ArrayList> RegistryValues;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool GetRegistryValues()
        {
            RegistryKey RootKey = null, InterfaceKey = null;
            string[] InterfaceList = null;
            try
            {
                RegistryKey HKLMKey = Registry.LocalMachine;
                if (null == (RootKey = HKLMKey.OpenSubKey(REGKEY_ROOT, RegistryKeyPermissionCheck.ReadSubTree)))
                {
                    return false;
                }

                if (null == (InterfaceList = RootKey.GetSubKeyNames()))
                {
                    return false;
                }

                foreach (string Interface in InterfaceList)
                {
                    if (null == (InterfaceKey = RootKey.OpenSubKey(Interface, RegistryKeyPermissionCheck.ReadSubTree)))
                    {
                        continue;
                    }
                    object tempValue = InterfaceKey.GetValue(REGKEY_VALUE_NAME, null);
                    if (null != tempValue)
                    {
                        ArrayList ValueList = new ArrayList();
                        foreach (string Values in (string[])tempValue)
                            ValueList.Add(Values);
                        RegistryValues.Add(Interface, ValueList);
                    }
                    else
                    {
                        RegistryValues.Add(Interface, null);
                    }
                    InterfaceKey.Close();
                }
                RootKey.Close();
                return true;
            }
            catch (Exception e)
            {
                if (null != RootKey)
                {
                    if (null != InterfaceKey) InterfaceKey.Close();
                    RootKey.Close();
                }
                RegistryValues.Clear();

                Console.WriteLine(Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool EnableDHCPUsingWMI()
        {
            ManagementClass NetworkConfiguiration = new ManagementClass("Win32_NetworkAdapterConfiguration");
            object Result = NetworkConfiguiration.InvokeMethod("EnableDHCP", null);
            Console.WriteLine("Enabling DHCP returned: " + Result.ToString());
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool DisableDHCPUsingWMI()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void ShowNetworkInterfaces()
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            Console.WriteLine("Interface information for {0}.{1}     ",
                    computerProperties.HostName, computerProperties.DomainName);
            if (nics == null || nics.Length < 1)
            {
                Console.WriteLine("  No network interfaces found.");
                return;
            }

            Console.WriteLine("  Number of interfaces .................... : {0}", nics.Length);
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                Console.WriteLine();
                Console.WriteLine(adapter.Description);
                Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length, '='));
                Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
                Console.WriteLine("  Physical Address ........................ : {0}",
                           adapter.GetPhysicalAddress().ToString());
                Console.WriteLine("  Operational status ...................... : {0}",
                    adapter.OperationalStatus);
                string versions = "";

                // Create a display string for the supported IP versions.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    versions = "IPv4";
                }
                if (adapter.Supports(NetworkInterfaceComponent.IPv6))
                {
                    if (versions.Length > 0)
                    {
                        versions += " ";
                    }
                    versions += "IPv6";
                }
                Console.WriteLine("  IP version .............................. : {0}", versions);
                //ShowIPAddresses(properties);

                // The following information is not useful for loopback adapters.
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                {
                    continue;
                }
                Console.WriteLine("  DNS suffix .............................. : {0}",
                    properties.DnsSuffix);

                string label;
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    IPv4InterfaceProperties ipv4 = properties.GetIPv4Properties();
                    Console.WriteLine("  MTU...................................... : {0}", ipv4.Mtu);
                    if (ipv4.UsesWins)
                    {

                        IPAddressCollection winsServers = properties.WinsServersAddresses;
                        if (winsServers.Count > 0)
                        {
                            label = "  WINS Servers ............................ :";
                            //ShowIPAddresses(label, winsServers);
                        }
                    }
                }

                Console.WriteLine("  DNS enabled ............................. : {0}",
                    properties.IsDnsEnabled);
                Console.WriteLine("  Dynamically configured DNS .............. : {0}",
                    properties.IsDynamicDnsEnabled);
                Console.WriteLine("  Receive Only ............................ : {0}",
                    adapter.IsReceiveOnly);
                Console.WriteLine("  Multicast ............................... : {0}",
                    adapter.SupportsMulticast);
                //ShowInterfaceStatistics(adapter);

                Console.WriteLine();
            }
        }
    }
}