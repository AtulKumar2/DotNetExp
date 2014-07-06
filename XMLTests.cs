using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DotNetExp
{
    class XMLTests
    {
        public void Run ()
        {
            searchXMLNode();
            return;
        }

        void searchXMLNode()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"C:\Temp\smallOutput\wmiInstanceData_1.xml");

            XmlNode nodeRoot = xmlDoc.DocumentElement;
            XmlNodeList listNSNodes = nodeRoot.ChildNodes;

            foreach (XmlNode xmlNode in listNSNodes)
            {
                Console.WriteLine(xmlNode.Attributes["Path"].Value.ToString());
            }

            return;
        }
    }
}
