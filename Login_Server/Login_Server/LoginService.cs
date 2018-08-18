using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Login_Server
{
    class LoginService
    {
        private TcpListener listener;

        private AutoResetEvent eventController;

        public LoginService()
        {
            eventController = new AutoResetEvent(false);
        }

        public void Start(string ip, int port) {
            try
            {
                if(ip == "localhost")
                    listener = new TcpListener(IPAddress.Any, port);
                else
                    listener = new TcpListener(IPAddress.Parse(ip), port);

                listener.Start();

            }catch(SystemException e)
            {
                Console.WriteLine("Listener가 제대로 생성되지 못했습니다.");
            }


        }

        private void Connect()
        {
            listener.;
        }
    }
}
