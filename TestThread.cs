using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Management;
using System.IO;


namespace DotNetExp
{
    class TestThread
    {
        private static EventWaitHandle ewMemProfilerRunning =
            new EventWaitHandle(false, EventResetMode.ManualReset);
        private static EventWaitHandle ewWMIQueryThreadsRunning =
            new EventWaitHandle(false, EventResetMode.ManualReset);
        private static EventWaitHandle ewExitProcessing =
            new EventWaitHandle(false, EventResetMode.ManualReset);
        const int totalWMIThread = 50;
        const int countWMIQueryRepeat = 1;
        private static long totalAliveWMIThread = 0;

        const string logMemProfiler = "MemProfilerLog.txt";
        const string logWMIQueryThreads = "WMIQueryThreadsLog.txt";
        private static Mutex muWMILogger = new Mutex();

        /// <summary>
        /// 
        /// </summary>
        [MTAThread]
        public void Run()
        {
            // Create Groups
            Console.WriteLine("MAIN: Finished creating groups " + Environment.NewLine);

            // Start Memory profiler thread
            //
            Thread tMemProf = new Thread(new ThreadStart(memProfilerThreadProc));
            tMemProf.Start();
            WaitHandle[] waitHandles = new WaitHandle[1];
            waitHandles.SetValue(ewMemProfilerRunning, 0);
            WaitHandle.WaitAny(waitHandles);
            Console.WriteLine("MAIN: Memory Profiler is now running " + Environment.NewLine);

            // Start WMI Query thread
            //
            Thread[] tWMIQuery = new Thread[totalWMIThread];
            for (int i = 0; i < totalWMIThread; i++)
            {
                tWMIQuery[i] = new Thread(new ThreadStart(wmiQueryThreadProc));
                tWMIQuery[i].Start();
            }
            tMemProf.Join();

            // TBD: Consolidate logs here in proper format.
            // 
            return;
        }

        /// <summary>
        /// Thread procedure to handle memory profiler operations.
        /// </summary>
        public static void memProfilerThreadProc()
        {
            string nl = Environment.NewLine;
            string nameProc = "WMIPRVSE";
            StreamWriter swMemProf = null;
            try
            {
                swMemProf = File.CreateText(logMemProfiler);
            }
            catch (System.UnauthorizedAccessException)
            {
                Console.WriteLine("System.UnauthorizedAccessException when creating Log file " + logMemProfiler);
                Console.WriteLine("Continue writing on console.");
                swMemProf = null;
            }
            catch (System.IO.PathTooLongException)
            {
                Console.WriteLine("System.IO.PathTooLongException when creating Log file " + logMemProfiler);
                Console.WriteLine("Continue writing on console.");
                swMemProf = null;
            }
            catch (Exception)
            {
                throw;
            }

            WaitHandle.SignalAndWait(ewMemProfilerRunning, ewWMIQueryThreadsRunning);
            getProcMem(nameProc, 0, swMemProf);

            logMemProf(swMemProf,
                String.Format("MemProf: Ready to take regular snapshot of {0}.", nameProc));
            // This is mainly to signal WMI thread which had earlier send its signal. 
            ewMemProfilerRunning.Set();

            TimeSpan waitTime = new TimeSpan(0, 0, 10);
            int cntSnapShot = 0;
            bool resultWait = false;

            DateTime lastUpdateTime = DateTime.Now;
            getProcMem(nameProc, cntSnapShot++, swMemProf);

            while (true)
            {
                getProcMem(nameProc, cntSnapShot++, swMemProf);
                lastUpdateTime = DateTime.Now;

                if (resultWait = ewExitProcessing.WaitOne(waitTime, true))
                {
                    if (1 < cntSnapShot)
                    {
                        getProcMem(nameProc, cntSnapShot++, swMemProf);
                        logMemProf(swMemProf, "MemProf: Received exit signal.");
                        break;
                    }
                    else
                    {
                        resultWait = false;
                    }
                    ewExitProcessing.Set(); // Set it so that we get it again.
                }
                if (resultWait) break;
            }
            logMemProf(swMemProf, "MemProf: Finished taking snapshot of WMIPRVSE.exe.");
            swMemProf.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameProc"></param>
        /// <param name="cntSnapShot"></param>
        /// <param name="swMemProf"></param>
        private static void getProcMem(string nameProc, int cntSnapShot, StreamWriter swMemProf)
        {
            Process[] listProc = Process.GetProcessesByName(nameProc);
            string data = "";
            foreach (Process proc in listProc)
            {
                data += String.Format("{0}:{1}: {2} MB{3}", proc.ProcessName, proc.Id,
                                        proc.PrivateMemorySize64 / (1024 * 1024), Environment.NewLine);
            }
            logMemProf(swMemProf,
                String.Format("MemProf: Snapshot# {0} of {1} @ {2}. {3}", cntSnapShot, nameProc,
                                       Environment.NewLine, data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="swMemProf"></param>
        /// <param name="logMessage"></param>
        private static void logMemProf(StreamWriter swMemProf, string logMessage)
        {
            if (null == swMemProf)
                Console.WriteLine(logMessage);
            else
            {
                swMemProf.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                swMemProf.WriteLine("  :{0}", logMessage);
                swMemProf.WriteLine("-------------------------------");
                // Update the underlying file.
                swMemProf.Flush();
            }
        }

        /// <summary>
        /// Thread procedure to handle WMI operations.
        /// </summary>
        private static StreamWriter swWMIQueryLog = null;
        public static void wmiQueryThreadProc()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            string threadLogString = "Thread <" + threadId + ">: "; 

            muWMILogger.WaitOne();
            try
            {
                if (null == swWMIQueryLog)
                    swWMIQueryLog = File.CreateText(logWMIQueryThreads);
            }
            catch (System.UnauthorizedAccessException)
            {
                Console.WriteLine(threadLogString + "System.UnauthorizedAccessException when creating Log file " + logWMIQueryThreads);
                Console.WriteLine(threadLogString + "Continue writing on console.");
                swWMIQueryLog = null;
            }
            catch (System.IO.PathTooLongException)
            {
                Console.WriteLine(threadLogString + "System.IO.PathTooLongException when creating Log file " + logWMIQueryThreads);
                Console.WriteLine(threadLogString + "Continue writing on console.");
                swWMIQueryLog = null;
            }
            catch (Exception)
            {
                muWMILogger.ReleaseMutex();
                throw;
            }
            muWMILogger.ReleaseMutex();

            logWMIThread(threadLogString + "Connecting to NS.");

            ManagementScope scope = new ManagementScope(@"\\.\root\cimv2");
            scope.Connect();

            Interlocked.Increment(ref totalAliveWMIThread);
            if (totalWMIThread == Interlocked.Read(ref totalAliveWMIThread))
                ewWMIQueryThreadsRunning.Set();

            WaitHandle[] waitHandles = new WaitHandle[1];
            waitHandles.SetValue(ewMemProfilerRunning, 0);
            EventWaitHandle.WaitAll(waitHandles);
            ewMemProfilerRunning.Set(); // Inform others waiting on this event

            for (int i = 0; i < countWMIQueryRepeat; i++)
            {
                //logWMIThread(threadLogString + "Iteration <" + countWMIQueryRepeat + ">");
                // Win32_Group
                //
                ManagementObjectSearcher mgmtSearcher = new ManagementObjectSearcher(scope,
                                new ObjectQuery("Select Domain, Name, SidType from Win32_Group"));
                int count = 0;
                foreach (ManagementObject mgmtObj in mgmtSearcher.Get())
                {
                    count++;
                    //Console.WriteLine(mgmtObj.Properties["Name"].Value.ToString());
                }
                //logWMIThread(threadLogString + String.Format("Win32_Group returned total {0} objects.", count));

                // Win32_Account
                //
                mgmtSearcher.Query = new ObjectQuery("Select * from Win32_Account");
                count = 0;
                foreach (ManagementObject mgmtObj in mgmtSearcher.Get())
                {
                    count++;
                    //Console.WriteLine(mgmtObj.Properties["Name"].Value.ToString());
                }
                //logWMIThread(threadLogString + String.Format("Win32_Account returned total {0} objects.", count));
            }
            
            logWMIThread(String.Format("{0} Finished Running all iterations.{1}", 
                                threadLogString, Environment.NewLine));
            Interlocked.Decrement(ref totalAliveWMIThread);
            if (0 == Interlocked.Read(ref totalAliveWMIThread))
            {
                logWMIThread(threadLogString + "All threads stopped. Sending close signal.");
                swWMIQueryLog.Close();
                ewExitProcessing.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logMessage"></param>
        private static void logWMIThread(string logMessage)
        {
            string nl = Environment.NewLine;
            string data = String.Format("{0} {1}: {2}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString(), logMessage);

            if (null == swWMIQueryLog)
            {
                Console.WriteLine(data);
            }
            else
            {
                muWMILogger.WaitOne();
                swWMIQueryLog.WriteLine(data);
                // Update the underlying file.
                swWMIQueryLog.Flush();
                muWMILogger.ReleaseMutex();
            }
        }
    }
}


//Main -> Create Account and Groups. Verify these are created.

//Main -> Start Memory Profiler Thread and Wait on MemProfRunning Signal for it to start and get ready.

//MemProf -> Signal to Main thread using MemProfRunning event that it has started. 
//Start Waiting on WMIThreadRunning Event.

//Main -> Come out of WAIT and then start WMI Thread. Start on both thread to be exited.
//If MemProf exits before WMI, exit application.

//WMI -> Be ready to fire Query. Connect to NS etc. 
//Signal to MemProf that WMI has started and start waiting on MemProfRunning Event.

//MemProf -> Take current snapshot of system and signal MemProfRunning to unblock WMI thread. 
//Take snapshot Every 20 seconds. If   ExitProcessing even has been signaled, then exit

//If ExitProcessing is received by any thread, 
//It resets it after using it so that other threads can get the same signal.