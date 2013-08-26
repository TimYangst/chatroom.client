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


namespace chatroom.client.ViewModel
{
	public class ChatRoomViewModel : WorkspaceViewModel
	{
		public ChatRoomViewModel()
		{
            CallViaDelegate();
		}


        private string fetchMessageList()
        {
            string rst = "";
            try
            {
                Encoding encoding = Encoding.UTF8;
                HttpWebRequest req = HttpWebRequest.Create("http://10.172.76.226:8888/list") as HttpWebRequest;
                req.Timeout = (20 * 1000);
                var resp = req.GetResponse() as HttpWebResponse;
                var stream = resp.GetResponseStream();
                byte[] buffer = new byte[256];
                StreamReader reader =  new StreamReader(stream,encoding);
                rst = reader.ReadToEnd();
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
            return rst;
        }


        private void CallViaDelegate()
        {
            Func<string> func = fetchMessageList;
            func.BeginInvoke(CallViaDelegateCallback, null);

        }

        private void CallViaDelegateCallback( IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string> func = (ar as AsyncResult).AsyncDelegate as Func<string>;
            try
            {
                string rst = func.EndInvoke(ar);
                Console.WriteLine(string.Format("{0} => {1}", str, rst));
                if (rst != null) {
                    JArray ja = (JArray)JsonConvert.DeserializeObject(rst);
                    foreach (var obj in ja) {
                        this.MessageList.Add(new Message(obj["content"].ToString(), obj["username"].ToString(), obj["time"].ToString()));
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
            string rst = "";
            try
            {
                HttpWebRequest req = HttpWebRequest.Create("http://10.172.76.226:8888/post") as HttpWebRequest;
                req.Method = "POST";
                req.Timeout = (20 * 1000);
                Encoding encoding = Encoding.UTF8;
                req.ContentType = "application/x-www-form-urlencoded";
                String str = String.Format("username={0}&content={1}","t-tiyan",msg);
                byte[] bs = Encoding.ASCII.GetBytes(str);
                req.ContentLength = bs.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                    reqStream.Close();
                }

                
                var resp = req.GetResponse() as HttpWebResponse;
                var stream = resp.GetResponseStream();
                byte[] buffer = new byte[256];
                StreamReader reader = new StreamReader(stream,encoding);
                rst = reader.ReadToEnd();
                resp.Close();
                return rst;

            }
            catch (WebException we)
            {
                Console.WriteLine("Get Response StatusCode: {0}({1})", we.Status, (int)we.Status);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
                    this.MessageList.Add(new Message(obj["content"].ToString(), obj["username"].ToString(),obj["time"].ToString()));
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