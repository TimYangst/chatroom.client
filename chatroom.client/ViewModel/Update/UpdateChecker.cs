using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using chatroom.client.ViewModel.Asynchronous;
using chatroom.client.ViewModel.Configuration;
using chatroom.client.ViewModel.Http;
using System.Xml;

namespace chatroom.client.ViewModel.Update
{
    public class UpdateChecker
    {

        private IntervalTaskRuntime CheckUpdateRuntime;

        private void checkUpdateDaemon()
        { 
            DateTime StartTime = DateTime.Now.AddSeconds(15);
            TimeSpan Interval = TimeSpan.FromSeconds(ConfigurationManager.UpdateCheckInterval);
            CheckUpdateRuntime = AsyncTaskExecuter.ExecuteIntervalTask(StartTime, Interval, getRemoteVersion, getVersionCallback);
        }

        private string getRemoteVersion()
        {
            return HttpRequestSender.sendRequest(ConfigurationManager.ServerUrl + "/update/version.txt", null, "GET");
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
            string remoteversion = getRemoteVersionNumber(versionXml);
            string localversion = getLocalVersionNumber();
            if (remoteversion == "") return;
            if (CheckUtils.CompareVersion(remoteversion, localversion))
            {

            }

        }

      

        #region GetVersionNumber

        private string getRemoteVersionNumber(String versionXml)
        {
            if (versionXml == null || versionXml == "") return "";
            try
            {
                XmlDocument remoteXml = new XmlDocument();
                remoteXml.LoadXml(versionXml);
                return getVersionNumber(remoteXml);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return "";
            }
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
