using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chatroom.client.Model
{
    public class Message
    {
        public Message(string content, string username , string time)
        {
            this.content = content;
            this.username = username;
            this._time = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime().AddMilliseconds(long.Parse(time)); 
            
        }
        public string content {get;set;}
        public string username {get;set;}

        private DateTime _time;
        public String time {
            get{
                return _time.ToLongDateString();
            }
        }
    }
}
