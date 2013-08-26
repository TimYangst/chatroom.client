using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using chatroom.client.Model;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;


namespace chatroom.client.ViewModel
{
	public class ChatRoomViewModel : WorkspaceViewModel
	{
        IntervalTaskRuntime heartBeatRuntime;

        private long lastTime = 0;

		public ChatRoomViewModel()
		{
            FetchMessageDelegate();
            InitHeartBeatTimer();
		}

        private void InitHeartBeatTimer()
        {
            DateTime start = DateTime.Now.AddSeconds(5);
            TimeSpan interval = TimeSpan.FromSeconds(5);
            heartBeatRuntime = TaskExecuter.ExecuteIntervalTask(start,interval,SendHeartBeat, ReceivedHeartBeatResponse);
        }

        private string SendHeartBeat() 
        {

            string rst = sendRequest("http://10.172.76.226:8888/heartbeat?lasttime=" + lastTime,null,"get"); 
            return rst;
        }

        private void ReceivedHeartBeatResponse(string rst)
        {
            Console.WriteLine(string.Format("{0} => {1}","receive", rst));
            if (rst != null)
            {
                JArray ja = (JArray)JsonConvert.DeserializeObject(rst);
                foreach (var obj in ja)
                {
                    AddToMessageList(obj as JObject);
                }

            }
        }
        private void AddToMessageList(JObject obj)
        {
            this.MessageList.Add(new Message(obj["content"].ToString(), obj["username"].ToString(), obj["time"].ToString()));
            long time = long.Parse(obj["time"].ToString());
            if (time > lastTime) lastTime = time;
        }


        private string sendRequest(string url, Dictionary<string, string> Paras,string method)
        {
            Console.WriteLine("send request => " + url);
            string content = "";
            try{
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest req = HttpWebRequest.Create(url) as HttpWebRequest;
                req.Timeout = 20 * 1000;
                if ("POST" == method || "post" == method)
                {
                    req.ContentType = "application/x-www-form-urlencoded";
                    StringBuilder sb = new StringBuilder();
                    bool first = true;
                    foreach (string key in Paras.Keys) 
                    {
                        if (!first) sb.Append('&');
                        sb.Append(String.Format("{0}={1}",key,Paras[key]));
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            } 
            return content;
        }


        private string fetchMessageList()
        {
            string rst = sendRequest("http://10.172.76.226:8888/list", null, "get");
            return rst;
        }

        private void FetchMessageDelegate()
        {
            Func<string> func = fetchMessageList;
            func.BeginInvoke(FetchMessageCallback, null);

        }

        
        private void FetchMessageCallback( IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string> func = (ar as AsyncResult).AsyncDelegate as Func<string>;
            try
            {
                string rst = func.EndInvoke(ar);
                Console.WriteLine(string.Format("{0} => {1}", str, rst));
                if (rst != null) {
                    JArray ja = (JArray)JsonConvert.DeserializeObject(rst);
                    foreach (var obj  in ja) {
                        AddToMessageList(obj as JObject);
                    }

                }
            }
            catch (Exception ex) {
                Console.WriteLine(string.Format("{0} => Error: {1}", str, ex.Message));
            }
        }


        private ObservableCollection<Message> _messageList = null;
        public ObservableCollection<Message> MessageList {
            get {
                if (_messageList == null) _messageList = new AsyncObservableCollection<Message>();
                return _messageList;
            }
            set {
                this._messageList = value;
                RaisePropertyChanged("MessageList");
            }
        }

        private string _message = "";
        public string Message {
            get {
                return _message;
            }
            set {
                if (_message == value) return;
                _message = value;
                RaisePropertyChanged("Message");
            }
        }

        private void sendMessage()
        {
            if (this.Message == "")
            {

            }
            else
            {
                String msg = this.Message;
                this.Message = "";
                SendMessageDelegate(msg);
            }
        }

        private string sendMessageRequest(string msg)
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras["username"] = "t-tiyan";
            paras["content"] = msg;
            string rst = sendRequest("http://10.172.76.226:8888/post", paras, "post");
            return rst;
        }

        private void SendMessageDelegate(string msg)
        {
            Func<string, string> func = sendMessageRequest;
            func.BeginInvoke(msg, SendMessageCallback, null);
        }

        private void SendMessageCallback(IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string,string> func = (ar as AsyncResult).AsyncDelegate as Func<string,string>;
            try
            {
                string rst = func.EndInvoke(ar);
                Console.WriteLine(string.Format("{0} => {1}", str, rst));
                if (rst != null)
                {
                    JObject obj = (JObject )JsonConvert.DeserializeObject(rst);
                    AddToMessageList(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0} => Error: {1}", str, ex.Message));
            }
        }

        private ICommand _sendMessage = null;
        public ICommand SendMessage {
            get {
                if (_sendMessage == null) _sendMessage = new RelayCommand(
                    this.sendMessage
                    );
                return _sendMessage;
            }
        }

		
	}
}