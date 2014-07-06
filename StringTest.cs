using System;
using System.Collections.Generic;
using System.Text;
using System.Management;

namespace DotNetExp
{
    class StringTest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            SplitCookie();
            //checkTrimEnd();
            //checkSplit();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SplitCookie()
        {
            // ASP.NET_SessionId=cm3bg4muwin1vmbovc1esjy3; host=SCANDICWEB101; ieAlertDisplay=true; scandic_lb_cookie=1702298890.0.0000; s_sv_sid=110733906247;
            String CookieString = "ASP.NET_SessionId=cm3bg4muwin1vmbovc1esjy3; host=SCANDICWEB101; ieAlertDisplay=true; scandic_lb_cookie=1702298890.0.0000; s_sv_sid=110733906247;";
            var parts = CookieString.Split(';');

            return;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool checkSplit()
        {
            string abc = "name = \\wmi\\abc\\rtd\\tre";
            Console.WriteLine(abc.Split('=')[1].Trim());
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool checkTrimEnd()
        {
            string path = "Y:\\";
            string path1 = path.TrimEnd('\\');

            Console.WriteLine("Path: " + path + Environment.NewLine + "path1: " + path1);
            return true;
        }
    }
}
