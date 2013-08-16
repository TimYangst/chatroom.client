using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace chatroom.client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closing += (o, e) =>
            {
                if (workThread != null) workThread.Abort();
            };
        }

        private Thread workThread = null;

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
            TextBlock block = this.FindName("m_output") as TextBlock;
            Int32 counter = 0;
            while (true)
            {
                SendUDP("127.0.0.1", 41181, counter.ToString(), counter.ToString().Length);
                Thread.Sleep(500);
                block.Text += "Sent: " + counter.ToString();
                block.Text += "\n";
                counter++;
            }
        }

        private void BeginToSend(object sender, RoutedEventArgs e)
        {
            if (workThread == null)
            {
                workThread = new Thread(runSender);
                workThread.Start();
            }
            else 
            {
                workThread.Resume();
            }
            (sender as Button).IsEnabled = false;
            (this.FindName("b_stop") as Button).IsEnabled = true;
        }

        private void EndToSend(object sender, RoutedEventArgs e)
        {
            workThread.Suspend();
            (sender as Button).IsEnabled = false;
            (this.FindName("b_start") as Button).IsEnabled = true;
        }

    }
}
