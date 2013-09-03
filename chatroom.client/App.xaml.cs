using chatroom.client.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;


namespace chatroom.client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        //event EventHandler hasupdatepackage;

    

        


        protected override void OnStartup(StartupEventArgs e)
        {

            UpdateChecker checker = new UpdateChecker();
            checker.NeedUpdate += checker_NeedUpdate;
            checker.startUpdateDaemon();

            //this.hasupdatepackage += App_hasupdatepackage; 

            this.MainWindow = new MainWindow();
            MainWindowViewModel MainWindowViewModel = new MainWindowViewModel();
            EventHandler handler = null;
            handler = delegate
            {
                MainWindowViewModel.RequestClose -= handler;
                MainWindow.Close();
            };
            MainWindowViewModel.RequestClose += handler;
            MainWindow.DataContext = MainWindowViewModel;
            MainWindow.Show();

        }
        /*
        private void App_hasupdatepackage(object sender, EventArgs e)
        {
          
        }*/

       
        private void checker_NeedUpdate(object sender, EventArgs e)
        {
            if (System.Windows.MessageBox.Show(App.Current.MainWindow, "已经完成新版本客户端下载，是否重启应用以完成更新?", "软件更新", MessageBoxButton.YesNo, MessageBoxImage.Information, MessageBoxResult.Yes) == MessageBoxResult.Yes)
            {
                //do shutdown app and run updater;
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName= "Update.exe";
                startInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath;

               // startInfo.

                Process.Start(startInfo);
                Application.Current.Shutdown();
            }

            //if (System.Windows.MessageBox.Show(Application.Current.MainWindow, "已经生成过，确定重新生成吗?", " 确 定 ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            //{

            //}
        }

    }

}
