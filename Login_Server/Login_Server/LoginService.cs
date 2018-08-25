using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;
using MySql.Data.MySqlClient;

namespace Login_Server
{
    class LoginService
    {
        // Listener
        private Socket listener;

        // EventArgs
        private SocketAsyncEventArgs accept_event;        

        // Pool
        private SocketAsyncEventArgsPool receive_args_pool;
        private SocketAsyncEventArgsPool send_args_pool;

        // 이거 send, receive 두개 만드는게 좋다.(굳이 그럴 필요 없음)
        private AutoResetEvent accept_event_control;

        // 통신에 사용되는 모든 buffer를 관리
        private BufferManager _bufferManager;

        // Listen 수용량
        private int _wait_capacity;

        public LoginService(int wait_capacity)
        {
            _wait_capacity = wait_capacity;

            Init();
        }

        private void Init()
        {
            accept_event = new SocketAsyncEventArgs();
            
            receive_args_pool = new SocketAsyncEventArgsPool(_wait_capacity);
            send_args_pool = new SocketAsyncEventArgsPool(_wait_capacity);

            accept_event.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessAccept);

            // 최대 동접자 수 * (recieve, send) * buffersize
            _bufferManager = new BufferManager(_wait_capacity * 2 * 1024, 1024);

            for(int i = 0; i<_wait_capacity; i++)
            {
                // receive
                {
                    SocketAsyncEventArgs temp_receive = new SocketAsyncEventArgs();
                    temp_receive.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessReceive);
                    temp_receive.UserToken = null;
                    // receive는 buffer를 사용.
                    _bufferManager.SetBuffer(temp_receive);
                    receive_args_pool.Push(temp_receive);
                }

                // send
                {
                    SocketAsyncEventArgs temp_send = new SocketAsyncEventArgs();
                    temp_send.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessSend);
                    temp_send.UserToken = null;
                    // send는 bufferList를 사용.
                    temp_send.BufferList = new List<ArraySegment<byte>>();
                    send_args_pool.Push(temp_send);
                }
            }
        }

        public void Start(string ip, int port)
        {
            IPEndPoint ipendpoint;

            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            if (ip == "localhost")
                ipendpoint = new IPEndPoint(IPAddress.Any, port);
            else
                ipendpoint = new IPEndPoint(IPAddress.Parse(ip), port);

            try
            {
                listener.Bind(ipendpoint);
                listener.Listen(_wait_capacity);

                Thread th = new Thread(StartAccept);
                // Background 처리해버리면 thread가 실행됨가 동시에 main thread가 종료되 프로세스가 종료된다.
                //th.IsBackground = true;
                th.Start();
            }
            catch(SystemException e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartAccept()
        {
            bool pending;

            accept_event_control = new AutoResetEvent(false);

            while (true)
            {
                Console.WriteLine("다른 연결 기다리는 중");
                accept_event.AcceptSocket = null;
                pending = false;

                try
                {
                    pending = listener.AcceptAsync(accept_event);
                }catch(SystemException e)
                {
                    Console.WriteLine(e);
                    continue;
                }

                accept_event_control.WaitOne();

                if (pending == false)
                    ProcessAccept(null, accept_event);
            }

        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.Success)
            {
                Console.WriteLine("Success to connect");

                Socket client = e.AcceptSocket;
                SocketAsyncEventArgs receive_args = receive_args_pool.Pop();
                SocketAsyncEventArgs send_args = send_args_pool.Pop();

                CToken token = new CToken(e.AcceptSocket, receive_args, send_args);
                receive_args.UserToken = token;
                send_args.UserToken = token;

                Receive(token);
            }
            else
            {
                Console.WriteLine("Fail to Connect");
            }

            accept_event_control.Set();
        }

        private void Receive(CToken token)
        {
            // 이부분처럼 버퍼를 셋 안해주면 오류 발생함.
            //token.receive_args.SetBuffer(new byte[1024], 0, 1024);
            //token.receive_args.UserToken = token;
                
            bool pending = token.client.ReceiveAsync(token.receive_args);

            if(pending == false)
            {
                ProcessReceive(null, token.receive_args);
            }
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            CToken token = e.UserToken as CToken;

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    token.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

                    if (token.isComplete == true)
                    {
                        Console.WriteLine("받을거 다 받았습니다.");

                        Send(token);
                        return;
                    }

                    bool pending = token.client.ReceiveAsync(e);

                    if(pending == false)
                    {
                        ProcessReceive(null, e);
                    }
                }
                else
                {
                    token.client.Close();
                }
            }
        }

        private void Send(CToken token)
        {
            List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
            list.Add(new ArraySegment<byte>(Encoding.ASCII.GetBytes(token.token)));
            list.Add(new ArraySegment<byte>(Encoding.ASCII.GetBytes(token.port)));

            token.send_args.BufferList = list;
            //token.send_args.UserToken = token;

            bool pending = token.client.SendAsync(token.send_args);

            if(pending == false)
            {
                ProcessSend(null, token.send_args);
            }
            
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError != SocketError.Success || e.BytesTransferred <= 0)
            {
                Console.WriteLine("Send 실패");
                return;
            }

            Console.WriteLine("전송 성공!");
            ((CToken)e.UserToken).client.Close();
            Console.WriteLine("기존 클라이언트와 연결 종료!");
        }
    }
}
