using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using HtmlAgilityPack; // http://htmlagilitypack.codeplex.com/
using Newtonsoft.Json;


namespace DotNetExp
{
    public class IMDBHelper
    {
        private readonly string imdbhelperinput = Environment.CurrentDirectory + "\\imdbhelperinput.txt";
        private readonly string imdbhelperoutput = Environment.CurrentDirectory + "\\imdbhelperoutput.txt";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public void Run(string [] args)
        {
            try
            {
                if (File.Exists(imdbhelperoutput))
                {
                    File.Delete(imdbhelperoutput);
                }
                
                StreamWriter output = new StreamWriter(imdbhelperoutput, false);

                using (StreamReader input = File.OpenText(imdbhelperinput))
                {
                    int count = 0;
                    while (input.Peek() >= 0)
                    {
                        string path = input.ReadLine().Trim();
                        Console.WriteLine("Getting data for " + path);
                        string [] pathtokens= path.Split(new char[]{'\\'});
                        IMDBHelper.imdbitem item = (new IMDBHelper()).GetInfoByTitle(pathtokens[pathtokens.Length-1]);

                        if (item == null)
                        {
                            Console.WriteLine("No data received for " + path);
                        }
                        else
                        {
                            output.WriteLine(path.Trim() + "\t" + item.year);
                        }
                        if ((count++ % 5) == 0) output.Flush();
                    }
                }
                output.Flush();
                output.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failure with exception {0}", e.ToString());
            }
            
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Title"></param>
        /// <returns></returns>
        private imdbitem GetInfoByTitle(string Title)
        {
            try
            {
                string url = "http://www.omdbapi.com/?i=&t=" + Title;
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "GET";
                req.UserAgent = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";
                string source;
                using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    source = reader.ReadToEnd();
                }
                dynamic Tokens = JsonConvert.DeserializeObject(source);
                imdbitem i = new imdbitem();
                i.year = Tokens.Year;
                i.release_date = Tokens.Released;
                return i;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception received. {0}", e.ToString());
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class imdbitem
        {
            public string rating { get; set; }
            public string rating_count { get; set; }
            public string year { get; set; }
            public string rated { get; set; }
            public string title { get; set; }
            public string imdb_url { get; set; }
            public string plot_simple { get; set; }
            public string type { get; set; }
            public string poster { get; set; }
            public string imdb_id { get; set; }
            public string also_known_as { get; set; }
            public string language { get; set; }
            public string country { get; set; }
            public string release_date { get; set; }
            public string filming_locations { get; set; }
            public string runtime { get; set; }
            public List<string> directors { get; set; }
            public List<string> writers { get; set; }
            public List<string> actors { get; set; }
            public List<string> genres { get; set; }
        }

    }
}

