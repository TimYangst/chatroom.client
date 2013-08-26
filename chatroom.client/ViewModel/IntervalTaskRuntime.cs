using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;

namespace chatroom.client.ViewModel
{
    public class IntervalTaskRuntime
    {
        private bool _keepRunning = true;

        public Func<string> Task;
        public Action<string> Callback;

        public void TimedThread(DateTime startTime, TimeSpan interval)
        {
            
            DateTime NextExecute = startTime;
            while (_keepRunning)
            {
                if (DateTime.Now > NextExecute)
                {
                    Task.BeginInvoke(TaskCallBack, null);
                    NextExecute = NextExecute.Add(interval);
                }
                //least interval
                Thread.Sleep(1000);
            }
        }

        private void TaskCallBack(IAsyncResult ar)
        {
            string str = (string)ar.AsyncState;
            Func<string> func = (ar as AsyncResult).AsyncDelegate as Func<string>;
            try
            {
                string rst = func.EndInvoke(ar);
                Callback.Invoke(rst);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("{0} => Error: {1}", str, ex.Message));
            }
        }
        private delegate void TaskRunner(DateTime startTime, TimeSpan interval);
        public void Invoke(DateTime startTime, TimeSpan interval)
        {
            TaskRunner runner = TimedThread;
            runner.BeginInvoke(startTime, interval, null, null);
        }

        public void Shutdown()
        {
            this._keepRunning = false;
        }
    }
}
