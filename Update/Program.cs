using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace chatroom.client.Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                var d = DateTime.Now;
                while (DateTime.Now.Subtract(d).TotalSeconds < 10) Application.DoEvents();
                XmlDocument document = UpdateUtils.LoadXmlFromFile("downloads/version.xml");
                if (document != null)
                {
                    string dversion = UpdateUtils.GetVersionNumberFromXml(document);
                    string dfilename = UpdateUtils.GetFileNameFromXml(document);
                    string lversion = UpdateUtils.GetVersionNumberFromFile("version.xml");
                    if (UpdateUtils.CompareVersion(dversion, lversion))
                    {
                        try
                        {
                            ZipUtils.UnZip("downloads/" + dfilename);
                            File.Delete("downloads/" + dfilename);
                            if (File.Exists("version.xml")) File.Delete("version.xml");
                            File.Move("downloads/version.xml", "version.xml");
                        }
                        catch (Exception ze)
                        {
                            Console.WriteLine(ze.StackTrace);
                        }

                    }
                }
                Process.Start("chatroom.client.exe");
                /*
                if (args.Contains("/r"))
                {
                    Process process = new Process();
                    process.StartInfo.FileName = "chatroom.client.exe";
                    process.Start();
                }*/

            }
            catch (Exception e) 
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
