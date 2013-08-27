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

        private static string _serverUrl = "";
        public static string ServerUrl
        {
            get { return _serverUrl; }
        }

        private static int _heartBeatInterval = 2000;
        public static int HeartBeatInterval 
        {
            get { return _heartBeatInterval;  }
        }

        private static string _userName = "unknow";
        public static string UserName {
            get { return _userName;  }
        }

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
                    if (ele.Name == "server") {
                        _serverUrl = ele.InnerText.Trim();
                    }
                    else if (ele.Name == "heartbeatinterval") {
                        _heartBeatInterval = int.Parse(ele.InnerText.Trim());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Load Config File Error:");
                Console.WriteLine(e.StackTrace);
                throw (new Exception("Load Config File Error:"));
            }

        }
        public static void GetEnvironmentArguements()
        {
            _userName = System.Environment.UserName;
        }

        static ConfigurationManager(){
            GetEnvironmentArguements();
            LoadConfigFile();
        }

    }
}
