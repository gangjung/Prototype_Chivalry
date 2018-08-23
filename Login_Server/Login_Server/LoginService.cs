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
        private Socket listener;
        private SocketAsyncEventArgs accept_event;
        private SocketAsyncEventArgs receive_event;
        private SocketAsyncEventArgs send_event;
        // 이거 send, receive 두개 만드는게 좋다.
        private AutoResetEvent autoResetEvent;
        private int _wait_capacity;

        public LoginService(int wait_capacity)
        {
            _wait_capacity = wait_capacity;
            accept_event = new SocketAsyncEventArgs();
            receive_event = new SocketAsyncEventArgs();
            receive_event.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessReceive);
            send_event = new SocketAsyncEventArgs();
            send_event.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessSend);
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

                accept_event.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptComplete);

                Thread th = new Thread(Listening);
                //th.IsBackground = true;
                th.Start();
            }
            catch(SystemException e)
            {
                Console.WriteLine(e);
            }
        }

        private void Listening()
        {
            bool pending;

            autoResetEvent = new AutoResetEvent(false);

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

                autoResetEvent.WaitOne();

                if (pending == false)
                    AcceptComplete(null, accept_event);
            }

        }

        private void AcceptComplete(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.Success)
            {
                Console.WriteLine("Success to connect");

                Socket client = e.AcceptSocket;
                CToken token = new CToken();
                token.client = client;

                e.UserToken = token;
                
                Receive(token);
            }
            else
            {
                Console.WriteLine("Fail to Connect");
            }

            autoResetEvent.Set();
        }

        private void Receive(CToken packet)
        {
            // 이부분처럼 버퍼를 셋 안해주면 오류 발생함.
            receive_event.SetBuffer(new byte[1024], 0, 1024);
            receive_event.UserToken = packet;
                
            bool pending = packet.client.ReceiveAsync(receive_event);

            if(pending == false)
            {
                ProcessReceive(null, receive_event);
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

        private void Send(CToken usertoken)
        {
            List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
            list.Add(new ArraySegment<byte>(Encoding.ASCII.GetBytes(usertoken.token)));
            list.Add(new ArraySegment<byte>(Encoding.ASCII.GetBytes(usertoken.port)));

            send_event.BufferList = list;
            send_event.UserToken = usertoken;

            bool pending = usertoken.client.SendAsync(send_event);

            if(pending == false)
            {
                ProcessSend(null, send_event);
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
