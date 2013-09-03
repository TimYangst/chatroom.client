using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace chatroom.client.Update
{
    public static class UpdateUtils
    {
        public static XmlDocument LoadXmlFromString(string xmlstring)
        {
            try
            {
                XmlDocument Xml = new XmlDocument();
                Xml.LoadXml(xmlstring);
                return Xml;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }

        public static XmlDocument LoadXmlFromFile(string filepath)
        {
            try
            {
                XmlDocument Xml = new XmlDocument();
                Xml.Load(filepath);
                return Xml;
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return null;
            }
        }


        public static string GetVersionNumberFromFile(string filepath)
        {
            try
            {
                XmlDocument localXml = LoadXmlFromFile(filepath);
                if (localXml == null) return "";
                return GetVersionNumberFromXml(localXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return "";
        }

        public static string GetFileNameFromXml(XmlDocument doc)
        {
            XmlNode node = doc.GetElementsByTagName("filename")[0];
            if (node == null) return "";
            return node.InnerText.Trim();
        }

        public static string GetVersionNumberFromXml(XmlDocument doc)
        {
            XmlNode node = doc.GetElementsByTagName("number")[0];
            if (node == null) return "";
            return node.InnerText.Trim();
        }
        /*
        * compare the version number;
        * the number format is in the format like X.XXX.XXX, the X is stand for a digit.
        * */
        public static bool CompareVersion(string a1, string a2)
        {
            if (a1 == null || a1.Trim() == "") return false;
            if (a2 == null || a2.Trim() == "") return true;
            try
            {
                a1 = a1.Trim();
                a2 = a2.Trim();
                int index1 = 0, index2 = 0;
                do
                {
                    index1 = a1.IndexOf('.');
                    index2 = a2.IndexOf('.');

                    int number1;
                    int number2;

                    if (index1 == -1) number1 = int.Parse(a1);
                    else
                    {
                        number1 = int.Parse(a1.Substring(0, index1));
                        a1 = a1.Substring(index1 + 1);
                    }

                    if (index2 == -1) number2 = int.Parse(a2);
                    else
                    {
                        number2 = int.Parse(a2.Substring(0, index2));
                        a2 = a2.Substring(index2 + 1);
                    }
                    if (number1 > number2) return true;
                    if (number2 < number1) return false;
                } while (index1 != -1 && index2 != -1);
                if (index1 != -1) return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return false;
        }

    }
}
