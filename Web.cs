using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace DotNetExp
{
    class Web
    {
        public void Run(String[] arg)
        {
            if (null != arg)
                Console.WriteLine("Arguments received [{0}]", String.Join(";", arg));
            ReadCookies();
            Console.ReadLine();
            return;
        }

        private void ReadCookies()
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create("http://gmail.com");
            request.CookieContainer = new CookieContainer();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            
            //Console.WriteLine(response.Headers.ToString());
            //Console.WriteLine(response.StatusCode.GetHashCode());

            CookieCollection IncomingCookies = response.Cookies;
            Console.WriteLine("Listing out {0} cookies received.", IncomingCookies.Count);
            foreach(Cookie cookie in IncomingCookies)
            {
                Console.WriteLine("{0} = {1}", cookie.Name, cookie.Value);
            }
            return;
        }
    }
}
