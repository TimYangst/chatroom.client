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
using chatroom.client.ViewModel.Configuration;


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
            InitHeartBeatTimer();
        }

        #endregion

        #region HeartBeatSender

        private long LastSynchronizationTime = 0;

        IntervalTaskRuntime heartBeatRuntime;

        private void InitHeartBeatTimer()
        {
            DateTime start = DateTime.Now.AddSeconds(1);
            TimeSpan interval = TimeSpan.FromMilliseconds(ConfigurationManager.HeartBeatInterval);
            heartBeatRuntime = AsyncTaskExecuter.ExecuteIntervalTask(start, interval, SendHeartBeat, ReceivedHeartBeatResponse);
        }

        private string SendHeartBeat()
        {

            string rst = HttpRequestSender.sendRequest(
                String.Format("{0}/heartbeat?lasttime={1}&username={2}",
                    ConfigurationManager.ServerUrl, 
                    LastSynchronizationTime,
                    ConfigurationManager.UserName), 
                null,
                "get");
            return rst;
        }

        private void ReceivedHeartBeatResponse(string rst)
        {
            Console.WriteLine(string.Format("{0} => {1}", "receive", rst));
            if (rst != null)
            {
                lock (x)
                {
                    JArray ja = (JArray)JsonConvert.DeserializeObject(rst);
                    foreach (var obj in ja)
                    {
                        AddToMessageList(obj as JObject, true);
                    }
                }

            }
        }

        #endregion

        #region MessageList

        private HashSet<long> messageset = new HashSet<long>();
        private static object x = new object();
        private void AddToMessageList(JObject obj, Boolean updatelast)
        {
        
                long time = long.Parse(obj["time"].ToString());
                if (!messageset.Contains(time))
                {
                    this.MessageList.Add(new Message(obj["content"].ToString(), obj["username"].ToString(), obj["time"].ToString()));
                    
                    messageset.Add(time);
                    
                }
                if (updatelast && time > LastSynchronizationTime) LastSynchronizationTime = time;
           
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
            paras["username"] = ConfigurationManager.UserName;
            paras["content"] = msg;
            string rst = HttpRequestSender.sendRequest(String.Format("{0}/post",ConfigurationManager.ServerUrl), paras, "post");
            return rst;
        }

        private void SendMessageCallback(string msg)
        {
            try
            {
                lock (x)
                {
                    if (msg != null)
                    {
                        JObject obj = (JObject)JsonConvert.DeserializeObject(msg);
                        AddToMessageList(obj, false);
                    }
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