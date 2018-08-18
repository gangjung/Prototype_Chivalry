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
    class Player
    {
        TcpClient tc;
        NetworkStream stream;

        Socket asdf;

        public Player(TcpClient tc, NetworkStream stream)
        {
            this.tc = tc;
            this.stream = stream;

            // 밑의 예제는 에러가 발생하는데, 그 이유는, Thread안에는 ThreadStart형식의 파라메터가 없는 델리게이트 형식이 들어가야하는데, 파라미터가 있는 함수의 델리게이트가 들어오면 ThreadStart형식으로 바꿀 수 없으므로 에러가 난다. 그래서 람다식으로 표현함으로써 ParameterizedThreadStart형으로 추론할 수 있게 한다.
            // Thread a = new Thread(Asdf(1, 2, 3));
            Thread a = new Thread(()=>Asdf(1, 2, 3));
            
        }

        public bool Read()
        {
            return true;
        }


        public void Asdf(int a, int b, int c) {

        }
    }
}
