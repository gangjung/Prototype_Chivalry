using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Login_Server
{
    class CToken
    {
        public Socket client;
        public SocketAsyncEventArgs receive_args;
        public SocketAsyncEventArgs send_args;

        public bool isComplete;
        public string token;
        public string port;

        private int HEADERSIZE;

        private string id;
        private string pw;

        private int type;

        private byte[] buffer;
        private int totalDataSize;
        private int currentIdx;

        // 유저 토큰
        public CToken(Socket client, SocketAsyncEventArgs receive_args, SocketAsyncEventArgs send_args)
        {
            this.client = client;
            this.receive_args = receive_args;
            this.send_args = send_args;

            isComplete = false;
            HEADERSIZE = 2;
            buffer = new byte[1024];
        }

        // 받았을 때
        public void OnReceive(byte[] buffer, int offset, int bytesize)
        {
            int bufferIdx = offset;
            int copysize = 0;
            int remainSize = bytesize;

            // 헤더파일을 읽고나서 남아있는 부분을 읽어오기 위해 반복문을 사용.
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
                    isComplete = true;
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

            // Login
            DBService dBService = new DBService();

            if(dBService.Login(id, pw, out token, out port) == false)
            {
                Console.WriteLine("Login 실패");
            }
        }

        public int GetTotalSize()
        {
            //Console.WriteLine(buffer);
            if (HEADERSIZE == 2)
                //return BitConverter.ToInt16(buffer, 0);
                return Int32.Parse(Encoding.ASCII.GetString(buffer));
            else if (HEADERSIZE == 4)
                return BitConverter.ToInt32(buffer, 0);

            return 0;
        }
    }
}
