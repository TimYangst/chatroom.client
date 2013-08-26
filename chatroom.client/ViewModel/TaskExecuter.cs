using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace chatroom.client.ViewModel
{
    /*
     * The task is in the form as : string taskname() {...};
     * And the callback use the result of task, then do sth.
     */
    public class TaskExecuter
    {
        public static IntervalTaskRuntime ExecuteIntervalTask(DateTime startTime, TimeSpan interval, Func<string> task, Action<string> callback)
        {
            IntervalTaskRuntime runtime =  new IntervalTaskRuntime();
            runtime.Task = task;
            runtime.Callback = callback;
            runtime.Invoke(startTime,interval);    
            return runtime;
        }


    }
}

