using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace chatroom.client.ViewModel
{
	public class ChatRoomViewModel : WorkspaceViewModel
	{
		public ChatRoomViewModel()
		{
			
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
                this.Message = "";
                Console.Out.WriteLine(this.Message);
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