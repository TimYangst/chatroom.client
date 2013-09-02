using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace chatroom.client.ViewModel.Configuration
{
    public static class ConfigurationManager
    {
        private static string CONFIG_FILE_PATH = "Config.xml";

        private static string SERVERURL = "server";
        private static string HEARTBEATINTERVAL = "heartbeatinterval";
        private static string UPDATECHECKINTERVAL = "updatecheckinterval";

        private static Dictionary<string, string> PropertiesTable = new Dictionary<string, string>();


        public static string ServerUrl
        {
            get { string outstring = "";
                if (PropertiesTable.TryGetValue(SERVERURL,out outstring)) 
                    return outstring;
                return "";
            }
        }

        public static int HeartBeatInterval 
        {
            get {
                string outstring = "";
                if (PropertiesTable.TryGetValue(HEARTBEATINTERVAL,out outstring))
                {
                    return int.Parse(outstring);
                }
                else return 5000;
            }
        }

        public static string UserName {
            get { return System.Environment.UserName; }
        }

        public static int UpdateCheckInterval 
        {
            get { string  outstring;
                if  (PropertiesTable.TryGetValue(UPDATECHECKINTERVAL, out  outstring)) return int.Parse(outstring);
                return 1800;
            }
        }

        #region initial

        public static void LoadConfigFile()
        {
            try
            {
                string PathToOpen = string.Format("{0}\\{1}",
                        System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), CONFIG_FILE_PATH);
                        
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(PathToOpen);
                XmlNode root = xmlDoc.SelectSingleNode("chatroom");
                foreach(XmlNode node in root.ChildNodes){
                    XmlElement ele = node as XmlElement;
                    PropertiesTable.Add(ele.Name, ele.InnerText.Trim());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Load Config File Error:");
                Console.WriteLine(e.StackTrace);
                throw (new Exception("Load Config File Error:"));
            }

        }

        static ConfigurationManager(){
            LoadConfigFile();
        }

        #endregion
    }
}
