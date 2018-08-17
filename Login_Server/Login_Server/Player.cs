using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Login_Server
{
    class Player
    {
        TcpClient tc;
        NetworkStream stream;

        Socket asdf;

        public Player(TcpClient tc, NetworkStream stream)
        {
            this.tc = tc;
            this.stream = stream;

            
        }

        public bool Read()
        {
            return true;
        }
    }
}
