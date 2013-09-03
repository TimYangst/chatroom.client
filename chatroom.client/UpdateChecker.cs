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
using System.Threading;
using chatroom.client.Update;

namespace chatroom.client
{
    public class UpdateChecker
    {
        private SynchronizationContext _synchronizationContext = SynchronizationContext.Current;

        private IntervalTaskRuntime CheckUpdateRuntime;
        
        public event EventHandler NeedUpdate;

        private string RemoteVersionNumber = "" ;

        private string PackageFileName = "";

        private void AskForUpdate(object param)
        {
      
                EventHandler handler = this.NeedUpdate;
                if (handler != null) handler(this, EventArgs.Empty);
          
        }


        private void DispatchUpdateInfo(string errorInfo)
        {
            if (errorInfo == null) {
                if (SynchronizationContext.Current == _synchronizationContext)
                {
                    AskForUpdate(null);
                }
                else {
                    _synchronizationContext.Post(AskForUpdate, EventArgs.Empty);
                }
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
            string localversion = UpdateUtils.GetVersionNumberFromFile("version.xml");
            if (UpdateUtils.CompareVersion(RemoteVersionNumber, localversion)) // step 2
            {
                if (PackageAlreadyDownload())
                {
                    DispatchUpdateInfo(null);
                }
                else 
                {
                    string str = DownloadThePackage();
                    DispatchUpdateInfo(str);
                }
            }
            else return;
        }

        private void ParseRemoteXml(String versionXml)
        {
            if (versionXml == null || versionXml == "") return ;
            try
            {
                XmlDocument remoteXml = UpdateUtils.LoadXmlFromString(versionXml);
                if (remoteXml == null) return;
                RemoteVersionNumber = UpdateUtils.GetVersionNumberFromXml(remoteXml);
                PackageFileName = UpdateUtils.GetFileNameFromXml(remoteXml);
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
            if (!Directory.Exists("downloads")) Directory.CreateDirectory("downloads");
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
            string dVersionNumber = UpdateUtils.GetVersionNumberFromFile("downloads/version.xml");
            if (dVersionNumber != RemoteVersionNumber) return false;
            if (PackageFileName == "") return false;
            return File.Exists("downloads/"+PackageFileName);
        }

       
    
    }
}
