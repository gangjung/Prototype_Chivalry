using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime.Serialization;

namespace Login_Server
{
    class Packet
    {
        public Socket client;

        private int HEADERSIZE;

        private string id;
        private string pw;

        private int type;

        private byte[] buffer;
        private int totalDataSize;
        private int currentIdx;

        public Packet()
        {
            HEADERSIZE = 2;
            buffer = new byte[1024];
        }

        public void OnReceive(byte[] buffer, int offset, int bytesize)
        {
            int bufferIdx = offset;
            int copysize = 0;
            int remainSize = bytesize;

            // 헤더파일을 읽고나서 남아있는 부분을 일거우기 위해 반복문을 사용.
            while (remainSize > 0)
            {
                if (currentIdx < HEADERSIZE)
                {
                    copysize = HEADERSIZE - currentIdx;

                    if (remainSize < copysize)
                        copysize = remainSize;

                    Array.Copy(buffer, bufferIdx, this.buffer, currentIdx, copysize);

                    bufferIdx += copysize;
                    currentIdx += copysize;

                    // 헤더를 다 안받아왔으므로 다시 receive해야한다.
                    if (currentIdx != HEADERSIZE)
                        return;

                    totalDataSize = GetTotalSize();
                    remainSize -= HEADERSIZE;
                }

                copysize = remainSize;

                if (copysize == 0)
                    return;

                Array.Copy(buffer, bufferIdx, this.buffer, currentIdx, copysize);
                currentIdx += copysize;

                if (currentIdx < totalDataSize)
                    return;
                else
                {
                    CompletePacket();
                    return;
                }
            }
        }

        // 콜백함수로 만들어서 다른 곳에 전달해줘도 상관없음.
        // MSDN을 참고하면서 확인해도 괜찮을 듯.
        public void CompletePacket()
        {
            string result = Encoding.ASCII.GetString(buffer, HEADERSIZE, totalDataSize - HEADERSIZE);

            string[] results = result.Split('/');
            id = results[0];
            pw = results[1];

            Console.WriteLine(id + "/" + pw);
        }

        public int GetTotalSize()
        {
            //Console.WriteLine(buffer);
            if (HEADERSIZE == 2)
                //return BitConverter.ToInt16(buffer, 0);
                return Int32.Parse( Encoding.ASCII.GetString(buffer));
            else if (HEADERSIZE == 4)
                return BitConverter.ToInt32(buffer, 0);

            return 0;
        }
    }

    class LoginService
    {
        private Socket listener;
        private SocketAsyncEventArgs accept_event;
        private SocketAsyncEventArgs receive_event;
        // 이거 send, receive 두개 만드는게 좋다.
        private AutoResetEvent autoResetEvent;
        private int _wait_capacity;

        public LoginService(int wait_capacity)
        {
            _wait_capacity = wait_capacity;
            accept_event = new SocketAsyncEventArgs();
            receive_event = new SocketAsyncEventArgs();
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
                Packet packet = new Packet();
                packet.client = client;

                e.UserToken = packet;
                
                Receive(packet);
            }
            else
            {
                Console.WriteLine("Fail to Connect");
            }

            autoResetEvent.Set();
        }

        private void Receive(Packet packet)
        {
            receive_event.Completed += new EventHandler<SocketAsyncEventArgs>(ProcessReceive);

            Console.WriteLine(packet);
            Console.WriteLine(packet.client);
            Console.WriteLine(receive_event);
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
            Packet packet = e.UserToken as Packet;

            if (e.LastOperation == SocketAsyncOperation.Receive)
            {
                if(e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    packet.OnReceive(e.Buffer, e.Offset, e.BytesTransferred);

                    bool pending = packet.client.ReceiveAsync(e);

                    if(pending == false)
                    {
                        ProcessReceive(null, e);
                    }
                }
                else
                {
                    packet.client.Close();
                }
            }
        }

        private void Send()
    }
}
