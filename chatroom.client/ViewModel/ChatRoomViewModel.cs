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
            return "[{\"username\":\"t-tiyan\",\"content\":\"hello!\",\"time\":1377241460939,\"_id\":\"521709744048d4342e000001\",\"__v\":0}]";
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
                    this.MessageList.Add(new Message("12334","yt"));
                }
            }
            catch (Exception ex) {
                Console.WriteLine(string.Format("{0} => Error: {1}", str, ex.Message));
            }
        }

        public ObservableCollection<Message> MessageList = new ObservableCollection<Message>();

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
                Console.Out.WriteLine(this.Message);
                this.Message = "";
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