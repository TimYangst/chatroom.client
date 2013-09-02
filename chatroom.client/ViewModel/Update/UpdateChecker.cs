using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using chatroom.client.ViewModel.Asynchronous;
using chatroom.client.ViewModel.Configuration;
using chatroom.client.ViewModel.Http;
using System.Xml;
using System.IO;
using System.Net;

namespace chatroom.client.ViewModel.Update
{
    public class UpdateChecker
    {

        private IntervalTaskRuntime CheckUpdateRuntime;
        
        public event EventHandler NeedUpdate;

        private string RemoteVersionNumber = "" ;

        private string PackageFileName = "";

        private void AskForUpdate(string arg)
        {
            if (arg == null)
            {
                EventHandler handler = this.NeedUpdate;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        public void startUpdateDaemon()
        { 
            DateTime StartTime = DateTime.Now.AddSeconds(15);
            TimeSpan Interval = TimeSpan.FromSeconds(ConfigurationManager.UpdateCheckInterval);
            CheckUpdateRuntime = AsyncTaskExecuter.ExecuteIntervalTask(StartTime, Interval, getRemoteVersion, getVersionCallback);
        }

        private string getRemoteVersion()
        {
            return HttpRequestSender.sendRequest(ConfigurationManager.UpdateServerUrl + "/update/version.xml", null, "GET");
        }

        /*
         * 1. get version number of remote server
         * 2. compare it with the version number of local running, if equal return.
         * 3. check if there is a package downloaded in the temporary directory.
         * 4. if there is not a package download the pacakge.
         * 5. ask user to update and restart.
         */
        private void getVersionCallback(String versionXml)
        {
            ParseRemoteXml(versionXml);
            string localversion = getLocalVersionNumber();
            if (CheckUtils.CompareVersion(RemoteVersionNumber, localversion)) // step 2
            {
                if (PackageAlreadyDownload())
                {
                    AskForUpdate(null);
                }
                else 
                {
                    AsyncTaskExecuter.ExecuteTask(DownloadThePackage, AskForUpdate);
                }
            }
            else return;
        }

        private void ParseRemoteXml(String versionXml)
        {
            if (versionXml == null || versionXml == "") return ;
            try
            {
                XmlDocument remoteXml = new XmlDocument();
                remoteXml.LoadXml(versionXml);
                
                RemoteVersionNumber = getVersionNumber(remoteXml);
                PackageFileName = getFileName(remoteXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return ;
            }
        }

        private string DownloadThePackage()
        {
            WebClient webClient = new WebClient();
            string packageUrl = ConfigurationManager.UpdateServerUrl + "/update/" + PackageFileName;
            try
            {
                webClient.DownloadFile(packageUrl, "downloads/" + PackageFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "Download package error.";
            }
            try
            {
                webClient.DownloadFile(ConfigurationManager.UpdateServerUrl + "/update/version.xml", "downloads/version.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "Download version.xml error.";
            }

            return null;
        }

        private bool PackageAlreadyDownload()
        {
            if (!File.Exists("downloads/version.xml")) return false;
            XmlDocument document = new XmlDocument();
            document.Load("downloads/version.xml");
            string dVersionNumber = getVersionNumber(document);
            if (dVersionNumber != RemoteVersionNumber) return false;
            if (PackageFileName == "") return false;
            return File.Exists("downloads/"+PackageFileName);
        }


        #region GetInfoFromXml

        private string getFileName(XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("filename");
            if (node == null) return "";
            return node.InnerText.Trim();
        }
    
        private string getLocalVersionNumber()
        {
            try
            {
                XmlDocument localXml = new XmlDocument();
                localXml.LoadXml("version.xml");
                return getVersionNumber(localXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);

            }
            return "";

        }

        private string getVersionNumber(XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("number");
            if (node == null) return "";
            return node.InnerText.Trim();
        }

        #endregion
    
    }
}
