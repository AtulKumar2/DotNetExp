using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DotNetExp
{
    struct shareData
    {
        public String name;
        public String path;
        public int type;
    }

    class StructDictionaryTest
    {
        Dictionary<String, ArrayList> shareDict;
        const int totalEntity = 4;
        const int totalInstances = 4; 

        /// <summary>
        /// 
        /// </summary>
        public StructDictionaryTest()
        {
            shareDict = new Dictionary<String, ArrayList>();
            for (int i = 0; i < totalInstances; i++)
            {
                shareDict.Add("Test" + i, new ArrayList());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            updateDict();
            dumpDict();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool updateDict()
        {
            shareData shareDataObj;

            for (int i = 0; i < totalEntity; i++)
            {
                for (int j = 0; j < totalInstances; j++)
                {
                    shareDataObj = new shareData();
                    shareDataObj.name = "Test" + i + "_Name_" + j;
                    shareDataObj.path = "\\\\path\\Test" + i + "_Name_" + j;
                    shareDataObj.type = 0;

                    shareDict["Test" + i].Add(shareDataObj);
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool dumpDict()
        {
            for (int i = 0; i < totalInstances; i++)
            {
                Console.WriteLine("Instance " + i + Environment.NewLine + "-----------");

                ArrayList shareList = shareDict["Test" + i];
                if (null != shareList && 0 != shareList.Count)
                {
                    foreach (shareData shareDataObj in shareList)
                    {
                        Console.WriteLine(shareDataObj.name + ":" +
                            shareDataObj.path + ":" + shareDataObj.type);
                    }
                }
            }
            return true;
        }
    }
}
