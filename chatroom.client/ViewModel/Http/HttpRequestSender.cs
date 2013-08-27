using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace chatroom.client.ViewModel.Http
{
    public class HttpRequestSender
    {
        public static string sendRequest(string url, Dictionary<string, string> Paras, string method)
        {
            Console.WriteLine("send request => " + url);
            string content = "";
            try
            {
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                req.Timeout = 20 * 1000;
                if ("POST" == method || "post" == method)
                {
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";

                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    foreach (string key in Paras.Keys)
                    {
                        if (!first) sb.Append('&');
                        sb.Append(String.Format("{0}={1}", key, Paras[key]));
                        first = false;
                    }
                    string querystring = sb.ToString();
                    byte[] bs = Encoding.ASCII.GetBytes(querystring);
                    req.ContentLength = bs.Length;
                    using (Stream reqStream = req.GetRequestStream())
                    {
                        reqStream.Write(bs, 0, bs.Length);
                        reqStream.Close();
                    }
                }
                var resp = req.GetResponse() as HttpWebResponse;
                var stream = resp.GetResponseStream();
                byte[] buffer = new byte[256];
                StreamReader reader = new StreamReader(stream, encoding);
                content = reader.ReadToEnd();
                resp.Close();
            }
            catch (WebException we)
            {
                Console.WriteLine("Get Response StatusCode: {0}({1})", we.Status, (int)we.Status);
                Console.WriteLine(we.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
          

            return content;
        }

    }
}
