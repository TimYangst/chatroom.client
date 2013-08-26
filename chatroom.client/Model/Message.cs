using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace chatroom.client.Model
{
    public class Message
    {
        public Message(string content, string username)
        {
            this.content = content;
            this.username = username;
            this.time = (long)(new DateTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalMilliseconds * 1000;
        }
        public string content {get;set;}
        public string username {get;set;}
        public long time {get;set;}
    }
}
