using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES_communicationApp
{

    class Message
    {
        public string name { get; set; }
        public string room { get; set; }
        public string msg { get; set; }
        public Message()
        {
            this.name = "daeun";
            this.room = "1";
            this.msg = "hello";
        }
        public Message(string name,string room,string msg)
        {
            this.name = name;
            this.room = room;
            this.msg = msg;
        }
    }
}
