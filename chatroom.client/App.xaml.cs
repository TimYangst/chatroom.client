using chatroom.client.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace chatroom.client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            this.MainWindow = new MainWindow();
            MainWindowViewModel MainWindowViewModel = new MainWindowViewModel();
            EventHandler handler =null;
            handler = delegate
            {
                MainWindowViewModel.RequestClose -= handler;
                MainWindow.Close();
            };
            MainWindowViewModel.RequestClose += handler;
            MainWindow.DataContext = MainWindowViewModel;
            MainWindow.Show();
        }
    }
    
}
