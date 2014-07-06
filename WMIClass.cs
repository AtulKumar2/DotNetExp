using System;
using System.IO;
using System.Collections.Generic;
using System.Management;
using System.Text;
using System.Collections.Specialized;
using System.Threading;

namespace DotNetExp
{
    class WMIClass
    {
        string nl;

        public bool Run()
        {
            nl = Environment.NewLine;

            //checkIfClassExists();
            //checkSelectQuery();
            //createDir();

            // GetRelatedClasses();

            // create and test shares START
            //SetUpforTrustee(null);
            //int returnValue;
            //TestForTrustee(null, null, null, 0, 0, null, null, out returnValue);
            // create and test shares END

            //createService();

            //allNonAbstractClasses(null);

            //getKeyQualifiers();

            //getNSObj();

            AssociationQuery();

            //namespaceNonAbstractClassesWithDerivation(@"root\cimv2");
            //checkPerfObjects("Win32_Share");
            //checkPerfObjects(@"\\ComputerName\root\cimv2:Win32_LogicalDisk");
            //checkPerfObjects(@"\\ComputerName\root\cimv2:Win32_PerfFormattedData_PerfDisk_PhysicalDisk.Name='_Total'");
            //checkPerfObjects(@"\\.\root\cimv2:Win32_PerfFormattedData_PerfDisk_PhysicalDisk.Name='_Total'");
            //checkPerfObjects("Win32_PerfFormattedData_PerfDisk_PhysicalDisk");

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        void AssociationQuery()
        {
            ManagementObjectCollection diskDrives = new ManagementObjectSearcher(
                        @"Select * From Win32_DiskDrive " +
                        "Where InterfaceType ='SCSI'").Get();

            if (0 == diskDrives.Count)
            {
                Console.WriteLine("Test will be skipped since no SATA drive is attached");
                return;
            }

            diskDrives = new ManagementObjectSearcher(
                        @"Select * From Win32_DiskDrive " +
                        "Where MediaType ='Removable Media'").Get();

            if (0 == diskDrives.Count)
            {
                Console.WriteLine("Test will be skipped since no USB drive is attached");
                return;
            }

            foreach (ManagementObject diskDrive in diskDrives)
            {
                ManagementObjectCollection pnpEntities =
                    new ManagementObjectSearcher
                    ("ASSOCIATORS OF {Win32_DiskDrive.DeviceID='" +
                    diskDrive["DeviceID"] + "'} " +
                    "WHERE AssocClass = Win32_PnPDevice").Get();

                foreach (ManagementObject pnpEntity in pnpEntities)
                {
                    foreach (PropertyData property in pnpEntity.Properties)
                    {
                        if (!property.IsArray)
                            Console.WriteLine("{0} : {1}",
                            property.Name,
                            property.Value);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PerformanceClass"></param>
        void checkPerfObjects(string PerformanceClass)
        {
            DateTime StartTime = DateTime.Now;
            ManagementObjectSearcher searcher = null;
            try
            {
                ManagementPath mgmtPath = new ManagementPath(PerformanceClass);
                if (mgmtPath.IsClass)
                {
                    if ("" == mgmtPath.NamespacePath)
                        searcher = new ManagementObjectSearcher(new SelectQuery(mgmtPath.ClassName));
                    else
                        searcher = new ManagementObjectSearcher(new ManagementScope(mgmtPath),
                                    new SelectQuery(mgmtPath.ClassName));
                }
                else if (mgmtPath.IsInstance)
                {
                    if ("" == mgmtPath.NamespacePath)
                        searcher = new ManagementObjectSearcher(new SelectQuery(mgmtPath.ClassName));
                    else
                        searcher = new ManagementObjectSearcher(new ManagementScope(mgmtPath),
                            new SelectQuery(mgmtPath.ClassName,
                            mgmtPath.RelativePath.Substring(mgmtPath.RelativePath.IndexOf('.') + 1)));
                }
                if (null != searcher)
                {
                    Console.WriteLine(searcher.Scope.Path.Server + ":" + searcher.Scope.Path.RelativePath
                                                + Environment.NewLine + searcher.Query.QueryString);
                }
                //return;
            }
            catch (ManagementException eM)
            {
                //if (ManagementStatus.InvalidQuery == eM.ErrorCode) 
                Console.WriteLine(eM.ToString());
                return;
            }

            for (int i = 1; i <= 1; i++)
            {
                Thread.Sleep(500);
                try
                {
                    Console.WriteLine(String.Format("Iteration {0} {1} --------", i, Environment.NewLine));
                    foreach (ManagementObject mgmtObj in searcher.Get())
                    {
                        //mgmtObj.Get();
                        string Message = mgmtObj.Properties["Name"].Value.ToString() + "{ ";
                        foreach (PropertyData propData in mgmtObj.Properties)
                        {
                            if (propData.Value != null)
                                Message += propData.Name + ":" + propData.Value.ToString() + " ; ";
                            else
                                Message += propData.Name + ": null ;";
                        }
                        Message += " }";
                        Console.WriteLine(Message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Class Enuumeration Exception." + Environment.NewLine +
                        searcher.Scope.Path.Server + ":" + searcher.Scope.Path.RelativePath + e.ToString());
                    return;
                }
            }
            TimeSpan totalTime = DateTime.Now.Subtract(StartTime);
            Console.WriteLine("Total Time taken = " + totalTime.ToString());
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        void getNSObj()
        {
            string path = @"\\.\ROOT\RSOP\User\";

            if ((path.IndexOf(@"\root", StringComparison.CurrentCultureIgnoreCase) + @"\root".Length == path.Length)
                || (path.IndexOf(@"\root\", StringComparison.CurrentCultureIgnoreCase) + @"\root\".Length == path.Length))
                return;

            int lastDelim = path.LastIndexOf(@"\");
            if (lastDelim == path.Length - 1)
            {
                path = path.Substring(0, lastDelim);
                lastDelim = path.LastIndexOf(@"\"); ;
            }
            string parentNS = path.Substring(0, lastDelim);

            ManagementObjectSearcher mgmtSearcher = new ManagementObjectSearcher(
                parentNS, String.Format("Select * from __NAMESPACE where Name='{0}'", path.Substring(lastDelim + 1)));

            foreach (ManagementObject mgmtObj in mgmtSearcher.Get())
            {
                //string[] splitName = mgmtObj.Path.RelativePath.Split('=');
                //if(0 == string.Compare(splitName[splitName.Length-1], path
                //if(string.Compare(mgmtObj.Path.NamespacePath.Substring(mgmtObj.Path.NamespacePath.LastIndexOf(@"\"))
                Console.WriteLine(mgmtObj.Path.NamespacePath);
                Console.WriteLine(mgmtObj.Properties["Name"].Value.ToString());
            }


            return;
        }

        /// <summary>
        /// Creates a share named Trutee on directory %systemdrive%\\win32_share directory with permission
        /// Check below to see what permissions has been set.
        /// </summary>
        void getKeyQualifiers()
        {
            ManagementClass mgmtClass = new ManagementClass(@"\\.\root\cimv2:Win32_COMApplicationClasses");
            //ManagementObjectCollection instColl = mgmtClass.Qualifiers["Key"]

            //StringCollection keyList = new StringCollection();

            //foreach (PropertyData prop in mgmtClass.Properties)
            //{
            //    foreach (QualifierData qual in prop.Qualifiers)
            //    {
            //        if (0 == string.Compare(qual.Name, "Key", true) &&
            //            0 == string.Compare(qual.Value.ToString(), "true", true))
            //        {
            //            keyList.Add(prop.Qualifiers["Key"].Name.ToString().ToLower());
            //        }
            //    }
            //}

            EnumerationOptions enumOpt = new EnumerationOptions();
            enumOpt.EnumerateDeep = true;
            //enumOpt.PrototypeOnly = true;
            enumOpt.Timeout = new TimeSpan(0, 1, 0);
            enumOpt.UseAmendedQualifiers = false;

            SelectQuery selQuery = new SelectQuery();
            selQuery.ClassName = mgmtClass.SystemProperties["__CLASS"].Value.ToString();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                mgmtClass.Scope,
                selQuery, enumOpt);

            ManagementObjectCollection mgmtColl = searcher.Get();

            foreach (ManagementObject mgmtObj in mgmtColl)
            {
                string value = "";
                foreach (PropertyData prop in mgmtObj.Properties)
                {
                    foreach (QualifierData qual in prop.Qualifiers)
                    {
                        if (0 == string.Compare(qual.Name, "Key", true) &&
                            0 == string.Compare(qual.Value.ToString(), "true", true))
                        {
                            value += prop.Name + ":" + prop.Value.ToString() + " ";
                        }
                    }
                }
                Console.WriteLine(value);
            }

            return;
        }

        /// <summary>
        /// Win32_Service class manipulation
        /// </summary>
        /// <returns></returns>
        bool createService()
        {
            ManagementClass serviceClass = new ManagementClass("Win32_Service");

            string strCurrDir = Environment.CurrentDirectory;
            string path = strCurrDir + @"\cimdatservice.exe";

            object[] methodArgs = { "jobobj", "jobobj", path, 16, 0, "Manual", false, null, null, null, null, null };

            object result = serviceClass.InvokeMethod("Create", methodArgs);
            Console.WriteLine("Creation of Service returned: " + result.ToString());

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shareName"></param>
        /// <returns></returns>
        bool SetUpforTrustee(string shareName)
        {
            string strSystemRoot = Environment.ExpandEnvironmentVariables("%COMPUTERNAME%");
            string NS = "\\\\" + strSystemRoot + "\\root\\cimv2";

            // Setup Win32_trustee
            ManagementClass clsTrustee = new ManagementClass(NS + ":" + "win32_trustee");
            ManagementObject objTrustee = clsTrustee.CreateInstance();
            string strUserDomain = Environment.ExpandEnvironmentVariables("%USERDOMAIN%");
            string strUserName = Environment.ExpandEnvironmentVariables("%USERNAME%");
            objTrustee.SetPropertyValue("Domain", strUserDomain);
            objTrustee.SetPropertyValue("Name", strUserName);

            // Setup Win32_account
            ManagementObject objAccount = new ManagementObject(NS
                            + ":" + "win32_Account.Name=\"" + strUserName
                            + "\",Domain=\"" + strUserDomain + "\"");
            string temp = objAccount.GetPropertyValue("SID").ToString();
            ManagementObject objSid = new ManagementObject(NS + ":" + "Win32_SID.SID=\"" + temp + "\"");
            ManagementClass clsAce = new ManagementClass(NS + ":" + "win32_Ace");
            ManagementObject objAce = clsAce.CreateInstance();
            objAce.SetPropertyValue("Trustee", objTrustee);
            objAce.SetPropertyValue("AccessMask", 0x1200a9);
            // SYNCHRONIZE 1048576 0x100000 + READ_CONTROL 131072 0x20000
            // FILE_READ_ATTRIBUTES 128 + FILE_TRAVERSE (directory) 32
            // FILE_READ_EA 8 + FILE_LIST_DIRECTORY 1

            objAce.SetPropertyValue("AceFlags", 0x4);   // NO_PROPAGATE_INHERIT_ACE
            objAce.SetPropertyValue("AceType", 0);      // Access allowed ACE

            //Setup win32_SecurityDescriptor
            ManagementObject[] arr = new ManagementObject[1];
            arr[0] = new ManagementObject();
            arr[0] = objAce;
            ManagementClass clsSec = new ManagementClass(NS + ":" + "win32_SecurityDescriptor");
            ManagementObject objSec = clsSec.CreateInstance();
            objSec.SetPropertyValue("Owner", objTrustee);
            objSec.SetPropertyValue("ControlFlags", 4);
            objSec.SetPropertyValue("DACL", arr);

            // Create directory for share
            DirectoryInfo diri = new DirectoryInfo(Environment.ExpandEnvironmentVariables("%SystemDrive%")
                                                + @"\Win32_Share");
            if (!diri.Exists)
            {
                diri.Create();
            }
            DirectoryInfo dir = new DirectoryInfo("C:\\Share");
            if (!dir.Exists)
            {
                dir.Create();
            }

            // Create share with Trustee information
            ManagementClass clsShare = new ManagementClass(NS + ":" + "win32_Share");
            ManagementBaseObject inParams = clsShare.GetMethodParameters("Create");
            inParams["Path"] = Environment.ExpandEnvironmentVariables("%SystemDrive%") + @"\Win32_Share";
            inParams["Name"] = "Trustee";
            inParams["Type"] = 0;
            inParams["MaximumAllowed"] = null;
            inParams["Description"] = "TrusteeTest";
            inParams["Password"] = null;
            inParams["Access"] = objSec;

            ManagementBaseObject outParams = clsShare.InvokeMethod("Create", inParams, null);
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceObject"></param>
        /// <param name="Path"></param>
        /// <param name="Name"></param>
        /// <param name="Type"></param>
        /// <param name="MaximumAllowed"></param>
        /// <param name="Description"></param>
        /// <param name="Password"></param>
        /// <param name="returnValue"></param>
        /// <returns></returns>
        int TestForTrustee(ManagementObject instanceObject, string Path, string Name,
                            int Type, int MaximumAllowed, string Description,
                            string Password, out int returnValue)
        {
            int Returnval;
            string strSystemRoot = Environment.ExpandEnvironmentVariables("%COMPUTERNAME%");
            string path = Environment.ExpandEnvironmentVariables("%SystemDrive%") + @" \Win32_Share.bat";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("net use m: \\\\" + strSystemRoot + "\\Trustee");
                }
            }
            path = Environment.ExpandEnvironmentVariables("%SystemDrive%") + @"\Win32_ShareResult.bat";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine("dir m:");
                }
            }

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            System.Diagnostics.Process proc1 = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemDrive%") + @"\Win32_Share.bat";
            proc.Start();
            proc.WaitForExit();
            proc1.StartInfo.FileName = Environment.ExpandEnvironmentVariables("%SystemDrive%") + @"\Win32_ShareResult.bat";
            proc1.Start();
            proc1.WaitForExit();

            Returnval = proc1.ExitCode;
            if (Returnval > 0)
            {
                returnValue = 0;
            }
            else
            {

                returnValue = 1;
            }
            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool GetRelatedClasses()
        {
            ManagementScope mgmtScope =
                            new ManagementScope("\\\\" + Environment.MachineName + "\\Root\\cimv2");
            ManagementPath mgmtPath = new ManagementPath("Win32_Share");
            ObjectGetOptions opt = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
            ManagementClass mgmtClass;

            try
            {
                mgmtClass = new ManagementClass(mgmtScope, mgmtPath, opt);


                Console.WriteLine(mgmtClass.ClassPath);
            }
            catch (ManagementException eM)
            {
                if (eM.ErrorCode == ManagementStatus.NotFound)
                {
                    Console.WriteLine("Class not found." + Environment.NewLine);
                    Console.WriteLine(eM.ToString());
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool checkSelectQuery()
        {
            String dirRoot = Directory.GetDirectoryRoot(Environment.SystemDirectory);
            dirRoot = dirRoot.TrimEnd('\\');

            SelectQuery selectQuery =
                new SelectQuery(String.Format(
                    "Select * from win32_directory where drive = '{0}' and path like '%jobobj%'", dirRoot));

            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

            foreach (ManagementObject mgmtObj in searcher.Get())
            {
                Console.WriteLine(mgmtObj.ToString());
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool checkIfClassExists()
        {
            ManagementScope mgmtScope =
                            new ManagementScope("\\\\" + Environment.MachineName + "\\Root\\cimv2");
            ManagementPath mgmtPath = new ManagementPath("Junkasbhgf");
            ObjectGetOptions opt = new ObjectGetOptions(null, TimeSpan.MaxValue, true);
            ManagementClass mgmtClass;

            try
            {
                mgmtClass = new ManagementClass(mgmtScope, mgmtPath, opt);

                Console.WriteLine(mgmtClass.ClassPath);
            }
            catch (ManagementException eM)
            {
                if (eM.ErrorCode == ManagementStatus.NotFound)
                {
                    Console.WriteLine("Class not found." + Environment.NewLine);
                    Console.WriteLine(eM.ToString());
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool createDir()
        {
            ManagementClass mgmtClass, mgmtClass_1;
            ManagementObject mgmtObj_1;

            try
            {
                ManagementScope mgmtScope =
                    new ManagementScope("\\\\" + Environment.MachineName + "\\root\\cimv2");

                ManagementPath mgmtPath = new ManagementPath("Win32_ProcessStartup");
                ObjectGetOptions opt = new ObjectGetOptions(null, TimeSpan.MaxValue, true);

                mgmtClass_1 = new ManagementClass(mgmtScope, mgmtPath, opt);
                mgmtObj_1 = mgmtClass_1.CreateInstance();
                mgmtObj_1.SetPropertyValue("ShowWindow", 0);

                mgmtPath = new ManagementPath("Win32_Process");
                mgmtClass = new ManagementClass(mgmtScope, mgmtPath, opt);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            try
            {
                int intProcessID = 0;
                object[] methodArgs = { "cmd.exe /c md " + "D:\\abc12", null, mgmtObj_1, intProcessID };
                object result = mgmtClass.InvokeMethod("Create", methodArgs);

                // Verify deletion operation
                if ("0" != result.ToString())
                {
                    Console.WriteLine("Creation comamnd for folder failed. Error <" + result.ToString() + ">.");
                    return false;
                }
                Console.WriteLine("Creation command for folder succedded.");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NS"></param>
        /// <returns></returns>
        bool allNonAbstractClasses(string NS)
        {
            try
            {
                ManagementScope mgmtScope;
                if (null == NS)
                {
                    mgmtScope = new ManagementScope(@"\\" + Environment.MachineName + @"\Root");
                }
                else
                {
                    mgmtScope = new ManagementScope(NS);
                }
                SelectQuery selectQuery =
                    new SelectQuery(String.Format("Select * from __NAMESPACE"));
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(mgmtScope, selectQuery);
                foreach (ManagementObject mgmtObj in searcher.Get())
                {
                    Console.WriteLine("==============================" + nl +
                        mgmtObj.ToString() + nl + "==============================");
                    namespaceNonAbstractClassesWithDerivation(mgmtScope.Path + @"\" +
                                                                mgmtObj.Properties["Name"].Value);
                    allNonAbstractClasses(mgmtScope.Path + @"\" + mgmtObj.Properties["Name"].Value);
                }
            }
            catch (ManagementException eM)
            {
                Console.WriteLine(eM.ErrorCode + nl + eM.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NS"></param>
        /// <returns></returns>
        bool namespaceNonAbstractClassesWithDerivation(string NS)
        {
            try
            {
                ManagementScope mgmtScope = new ManagementScope(NS);
                SelectQuery selectQuery =
                    new SelectQuery(String.Format("Select * from meta_class"));
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(mgmtScope, selectQuery);

                foreach (ManagementObject mgmtObj in searcher.Get())
                {
                    if (mgmtObj.Properties["__CLASS"].Value.ToString().Equals("CIM_SERVICE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int j = 10;
                    }
                    if (mgmtObj.Qualifiers.Count != 0)
                    {
                        bool bAbstractClass = false;
                        foreach (QualifierData qualifierData in mgmtObj.Qualifiers)
                        {
                            if (qualifierData.Name.ToString().Equals("Abstract", StringComparison.InvariantCultureIgnoreCase))
                            {
                                if (qualifierData.Value.ToString().Equals("true", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    bAbstractClass = true;
                                    break;
                                }
                            }
                        }
                        if (bAbstractClass) continue;
                    }
                    ManagementClass mgmtClass = new ManagementClass(new ManagementPath(
                                    mgmtObj.Properties["__PATH"].Value.ToString()));

                    string className = mgmtObj.Properties["__CLASS"].Value.ToString();
                    if (false == className.StartsWith("__"))
                    {
                        ManagementObjectCollection mgmtDerivedColl = mgmtClass.GetSubclasses();
                        if (0 != mgmtDerivedColl.Count)
                        {
                            Console.WriteLine(className);
                            //foreach (ManagementObject mgmtDer in mgmtDerColl)
                            //{
                            //    Console.WriteLine("\t" + mgmtDer.Properties["__CLASS"].Value.ToString());
                            //}
                        }
                    }
                }
            }
            catch (ManagementException eM)
            {
                Console.WriteLine(eM.ErrorCode + nl + eM.Message);
            }
            return true;
        }

    }
}
