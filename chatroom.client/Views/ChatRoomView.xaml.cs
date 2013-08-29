using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using chatroom.client.Model;
using System.Collections.Specialized;
using chatroom.client.ViewModel;

namespace chatroom.client
{
	/// <summary>
	/// ChatRoomView.xaml 的交互逻辑
	/// </summary>
	public partial class ChatRoomView : UserControl
	{
		public ChatRoomView()
		{
			this.InitializeComponent();
            
            ListView list = this.FindName("m_MessageList") as ListView;
            ObservableCollection<Message> collection = list.ItemsSource as ObservableCollection<Message>;
            collection.CollectionChanged += OnCollectionChanged;
		}

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListView listView = this.FindName("m_MessageList") as ListView;
            listView.ScrollIntoView(listView.Items[listView.Items.Count - 1]);
        }

        private void m_message_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                ChatRoomViewModel viewmodel = textBox.DataContext as ChatRoomViewModel;
                viewmodel.MessageToSend = textBox.Text;
                viewmodel.CheckAndSendMessage();
            }
        }
	}
}