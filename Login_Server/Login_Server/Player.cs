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
    class Login_Service
    {
        Socket listen_socket;
        SocketAsyncEventArgs accept_event;
        AutoResetEvent event_controll;

        Socket asdf;

        public Login_Service()
        {
            accept_event = null;
        }

        public void Start(string ip, int port)
        {
            IPEndPoint iPEndPoint;

            listen_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (ip == "localhost")
                iPEndPoint = new IPEndPoint(IPAddress.Any, port);
            else
                iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                listen_socket.Bind(iPEndPoint);
                listen_socket.Listen(100);

                accept_event = new SocketAsyncEventArgs();
                accept_event.Completed += new EventHandler<SocketAsyncEventArgs>(Accept_event_complete);

                // Thread로 실행시켜주는 이유는, 혹시라도 메인Thread에서 입력을 받게 된다면, listen이 중지될 수 있기에, 서브Thread로 빼준다.
                new Thread(Listening).Start();
                    
            }catch(SystemException e)
            {
                Console.WriteLine("소켓이 제대로 생성되지 못했습니다.");
            }
        }

        private void Listening()
        {
            bool pending;

            event_controll = new AutoResetEvent(false);
            

            while (true)
            {
                Console.WriteLine("다른 연결 기다리는 중");
                accept_event.AcceptSocket = null;
                pending = false;

                try
                {
                    // 동기적으로 처리되었을 시, false를 반환
                    pending = listen_socket.AcceptAsync(accept_event);
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                event_controll.WaitOne();

                if (pending == false)
                    Accept_complete(accept_event);
            }
        }

        // 왜 따로 만들어줬나??? 좀 더 명확하게 함수를 판단하고 사용하게 하기 위해서 ㅇㅇ
        private void Accept_event_complete(object sender, SocketAsyncEventArgs e)
        {
            Accept_complete(e);
        }

        // SocektAsyncEventArgs는 단순히 이벤트를 등록하는게 아니라, Async작업 후, 결과를 저장하는 장소로도 사용된다. 그렇기에 해당 변수를 넘겨받아 결과작업을 처리해줘야 한다.
        private void Accept_complete(SocketAsyncEventArgs e)
        {
            Console.WriteLine("클라이언트와 연결되었습니다.");

            if(e.SocketError == SocketError.Success)
            {
                Socket client = e.AcceptSocket;

                // to do : 클라이언트 연결 처리 부분
            }
            else
            {
                Console.WriteLine("Fail to connect Client");
            }

            event_controll.Set();
        }
    }
}
