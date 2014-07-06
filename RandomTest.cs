using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;

namespace DotNetExp
{
    class RandomTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            //findEndVariables();
            //writetoEventLog();
            //getDateInCIMDATETIMEFormat();
            //Console.WriteLine(RegistryExpansion());
            getRegKeyValue();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool getDateInCIMDATETIMEFormat()
        {
            //const string timeFmt = "{0,-30}{1:yyyyMMdd000000.000000}";
            System.DateTime today = System.DateTime.Now;
            TimeZone localZone = TimeZone.CurrentTimeZone;

            string strCIMFormat = today.Year.ToString();
            
            if (today.Month < 10)
                strCIMFormat += "0" + today.Month.ToString();
            else
                strCIMFormat += today.Month.ToString();

            if (today.Day < 10)
                strCIMFormat += "0" + today.Day.ToString();
            else
                strCIMFormat += today.Day.ToString();

            strCIMFormat += "000000.000000";

            if (localZone.GetUtcOffset(today).ToString()[0] == '-')
                strCIMFormat += "-" + localZone.GetUtcOffset(today).TotalMinutes;
            else
                strCIMFormat += "+" + localZone.GetUtcOffset(today).TotalMinutes;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool findEndVariables()
        {
            string strCurrDrive = Environment.CurrentDirectory;
            Console.WriteLine("strCurrDrive: " + strCurrDrive);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool writetoEventLog()
        {
            //EventLog.WriteEntry(
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string RegistryExpansion()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID\\{534A1E02-D58F-44f0-B58B-36CBED287C7C}");
            object keyValue = key.GetValue("DllSurrogate", null, RegistryValueOptions.DoNotExpandEnvironmentNames);
            if (keyValue == null)
            {
                Console.WriteLine("No Data");
            }
            return (keyValue.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool getRegKeyValue()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\AppID\\" 
                        + "{48da6741-1bf0-4a44-8325-293086c79077}");
            object keyValue = key.GetValue("ActivateAtStorage", null);
            if (keyValue == null)
            {
                Console.WriteLine("false");
                return false; 
            }
            else
            {
                Console.WriteLine("true");
                return true; 
            }
        }
    }
}
