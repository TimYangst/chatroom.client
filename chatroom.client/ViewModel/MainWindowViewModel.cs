using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.Net;
using System.Net.Sockets;
using GalaSoft.MvvmLight.Command;


namespace chatroom.client.ViewModel
{
    public class MainWindowViewModel : WorkspaceViewModel
    {
        public MainWindowViewModel()
        {
            this.RequestClose += (o,e) => AbortThread();
        }

        private string _logs = "";
        public string Logs {
            get { return _logs; }
            set {
                this._logs = value; 
                RaisePropertyChanged("Logs"); 
            }
        }

        private Thread workThread = null;

        private bool _started =  false;
        public bool Started {
            get { return _started; }
            set { 
                if (this._started != value) 
                {
                    this._started = value; 
                    RaisePropertyChanged("Started");
                } 
            }
        }

        private ICommand _startCommand = null;
        public ICommand StartCommand
        {
            get 
            {
                if (_startCommand == null)
                {
                    _startCommand = new RelayCommand(
                        this.BeginToSend,
                        () => (!this.Started)           
                        );
                    
                }
                return _startCommand;
            }
        }
        private ICommand _stopCommand = null;
        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                {
                    _stopCommand = new RelayCommand(
                        this.StopToSend,
                        () => (this.Started)
                        );
                }
                return _stopCommand;
            }
        }

      

        private void AbortThread()
        {
            if (workThread != null && workThread.IsAlive)
            {
                workThread.Resume();
                workThread.Abort();
            }
        }



        private void BeginToSend()
        {
            this.Started = true;
            if (workThread == null)
            {
                workThread = new Thread(runSender);
                workThread.IsBackground = true;
                workThread.Start();
            }
            else
            {
                workThread.Resume();
            }
           
        }
        private void StopToSend()
        {
            this.Started = false;
            workThread.Suspend();
            
        }

        private void SendUDP(string hostNameOrAddress, int destinationPort, string data, int count)
        {
            IPAddress destination = Dns.GetHostAddresses(hostNameOrAddress)[0];
            IPEndPoint endPoint = new IPEndPoint(destination, destinationPort);
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            for (int i = 0; i < count; i++)
            {
                socket.SendTo(buffer, endPoint);
            }
            socket.Close();

        }

        private void runSender()
        {
           
            Int32 counter = 0;
            while (true)
            {
                SendUDP("127.0.0.1", 41181, counter.ToString(), counter.ToString().Length);
                Thread.Sleep(500);
                this.Logs = this.Logs + ("Sent: " + counter.ToString() + "\n");
                counter++;
            }
        }

    }
}
