using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace chatroom.client.ViewModel.Asynchronous
{
    public class SinglePassTaskRuntime
    {
        public Action<String> Callback;

        public void Invoke(Func<string> task)
        {
            task.BeginInvoke(TaskCallBack1, null);
        }

        public void Invoke(Func<string, string> task, string param)
        {
            task.BeginInvoke(param, TaskCallBack2, null);
        }

        private void TaskCallBack1(IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string> func = (ar as AsyncResult).AsyncDelegate as Func<string>;
            try{
                string rst = func.EndInvoke(ar);
                Callback.Invoke(rst);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run AsyncTask Error :");
                Console.WriteLine(ex.StackTrace);
            }
           
        }

        private void TaskCallBack2(IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string,string> func = (ar as AsyncResult).AsyncDelegate as Func<string,string>;
            try
            {
                string rst = func.EndInvoke(ar);
                Callback.Invoke(rst);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run AsyncTask Error :");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
