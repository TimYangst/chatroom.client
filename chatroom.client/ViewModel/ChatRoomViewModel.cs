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
using chatroom.client.ViewModel.Http;
using chatroom.client.ViewModel.Asynchronous;


namespace chatroom.client.ViewModel
{
    public class ChatRoomViewModel : WorkspaceViewModel
    {

        #region Initialization

        public ChatRoomViewModel()
        {
            if (IsInDesignMode || IsInDesignModeStatic)
            {
                return;
            }
            AsyncFetchMessageHistory();
            InitHeartBeatTimer();
        }

        #endregion

        #region HeartBeatSender

        private long LastSynchronizationTime = 0;

        IntervalTaskRuntime heartBeatRuntime;

        private void InitHeartBeatTimer()
        {
            DateTime start = DateTime.Now.AddSeconds(5);
            TimeSpan interval = TimeSpan.FromSeconds(10);
            heartBeatRuntime = AsyncTaskExecuter.ExecuteIntervalTask(start, interval, SendHeartBeat, ReceivedHeartBeatResponse);
        }

        private string SendHeartBeat()
        {

            string rst = HttpRequestSender.sendRequest(String.Format("http://10.172.76.226:8888/heartbeat?lasttime={0}&username={1}", LastSynchronizationTime, "t-tiyan"), null, "get");
            return rst;
        }

        private void ReceivedHeartBeatResponse(string rst)
        {
            Console.WriteLine(string.Format("{0} => {1}", "receive", rst));
            if (rst != null)
            {
                JArray ja = (JArray)JsonConvert.DeserializeObject(rst);
                foreach (var obj in ja)
                {
                    AddToMessageList(obj as JObject, true);
                }

            }
        }

        #endregion

        #region AsyncFetchMessageHistory

        private void AsyncFetchMessageHistory()
        {
            try
            {
                AsyncTaskExecuter.ExecuteTask(fetchMessageList, fetchMessageListCallback);
            }
            catch (Exception e)
            {
                Console.WriteLine("Fetch Message History failed => {0}", e.ToString());
            }
        }

        private string fetchMessageList()
        {
            string rst = HttpRequestSender.sendRequest("http://10.172.76.226:8888/list", null, "get");
            return rst;
        }

        private void fetchMessageListCallback(string messagelist)
        {
            try
            {
                if (messagelist != null)
                {
                    JArray ja = (JArray)JsonConvert.DeserializeObject(messagelist);
                    foreach (var obj in ja)
                    {
                        AddToMessageList(obj as JObject, true);
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("parse json faild => Error: {1}", ex.Message));
            }
        }

        #endregion

        #region MessageList

        private HashSet<long> messageset = new HashSet<long>();
        private static object x = new object();
        private void AddToMessageList(JObject obj, Boolean updatelast)
        {
            lock (x)
            {
                long time = long.Parse(obj["time"].ToString());
                if (!messageset.Contains(time))
                {
                    this.MessageList.Add(new Message(obj["content"].ToString(), obj["username"].ToString(), obj["time"].ToString()));
                    
                    messageset.Add(time);
                    
                }
                if (updatelast && time > LastSynchronizationTime) LastSynchronizationTime = time;
            }
        }

        private ObservableCollection<Message> _messageList = null;
        public ObservableCollection<Message> MessageList
        {
            get
            {
                if (_messageList == null) _messageList = new AsyncObservableCollection<Message>();
                return _messageList;
            }
            set
            {
                this._messageList = value;
                RaisePropertyChanged("MessageList");
            }
        }

        #endregion

        #region MessageToSend

        private string _messageToSend = "";
        public string MessageToSend
        {
            get
            {
                return _messageToSend;
            }
            set
            {
                if (_messageToSend == value) return;
                _messageToSend = value;
                RaisePropertyChanged("MessageToSend");
            }
        }

        #endregion

        #region SendMessageCommand

        private ICommand _sendMessage = null;
        public ICommand SendMessage
        {
            get
            {
                if (_sendMessage == null) _sendMessage = new RelayCommand(
                    this.CheckAndSendMessage
                    );
                return _sendMessage;
            }
        }

        private void CheckAndSendMessage()
        {
            if (this.MessageToSend == "")
            {

            }
            else
            {
                String msg = this.MessageToSend;
                this.MessageToSend = "";
                AsyncSendMessage(msg);

            }
        }

        #endregion

        #region AsyncSendMessage
        private void AsyncSendMessage(string msg)
        {
            AsyncTaskExecuter.ExecuteTask(sendMessageRequest, msg, SendMessageCallback);
        }

        private string sendMessageRequest(string msg)
        {
            Dictionary<string, string> paras = new Dictionary<string, string>();
            paras["username"] = "t-tiyan";
            paras["content"] = msg;
            string rst = HttpRequestSender.sendRequest("http://10.172.76.226:8888/post", paras, "post");
            return rst;
        }

        private void SendMessageCallback(string msg)
        {
            try
            {
                if (msg != null)
                {
                    JObject obj = (JObject)JsonConvert.DeserializeObject(msg);
                    AddToMessageList(obj, false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Parse Json obj failed => Error: {1}", ex.Message));
            }
        }
        #endregion

    }
}