using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T_TalkS
{
    class Room
    {
        public string roomName;
        public string roomType;
        public string password;

        public Room(string rn = "", string rt = "", string rp = "")
        {
            roomName = rn;
            roomType = rt;
            password = rp;
        }
    }
}
