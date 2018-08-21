using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Login_Server
{
    class SocketAsyncEventArgsPool
    {
        public int Count { get { return pool.Count; } }

        private Stack<SocketAsyncEventArgs> pool;

        public SocketAsyncEventArgsPool(int capacity = 0)
        {
            if(capacity == 0)
                pool = new Stack<SocketAsyncEventArgs>();
            else
                pool = new Stack<SocketAsyncEventArgs>(capacity);
        }

        public void Push(SocketAsyncEventArgs item)
        {
            if(item == null)
            {
                throw new ArgumentException("You push invaild item. You shoud push correct item");
            }
            lock (pool)
            {
                pool.Push(item);
            }
        }

        public SocketAsyncEventArgs Pop()
        {
            if(pool.Count == 0)
            {
                throw new ArgumentException("Pool is empty");
            }
            lock (pool)
            {
                return pool.Pop();
            }
        }
    }
}
