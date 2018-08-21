using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Login_Server
{
    class BufferManager
    {
        private Stack<int> _freeBufferPool;
        private byte[] _buffer;
        private int _totalSize;
        private int _bufferSize;
        private int _currentIndex;
        private int _count;

        // 싱글톤으로 짜볼까?
        public BufferManager(int totalsize, int buffersize)
        {
            _totalSize = totalsize;
            _bufferSize = buffersize;
            _currentIndex = 0;
            _count = 0;
            _buffer = new byte[totalsize];
            _freeBufferPool = new Stack<int>();
        }

        public void InitBuffer()
        {
            _buffer = new byte[_totalSize];
        }

        // Lock 걸 필요가 없다. 왜냐하면, 처음 SocketAsyncEventArgsPool 만들 때 사용되고, 다른 곳에서는 사용되지 않기 때문!
        public bool SetBuffer(SocketAsyncEventArgs value)
        {
            if (value == null)
                throw new ArgumentException("Parameter is null");

            if(_freeBufferPool.Count == 0)
            {
                if (_currentIndex + _bufferSize > _totalSize)
                    return false;

                value.SetBuffer(_buffer, _currentIndex, _bufferSize);
                _currentIndex += _bufferSize;
            }
            else
            {
                value.SetBuffer(_buffer, _freeBufferPool.Pop(), _bufferSize);
            }

            return true;
        }

        public bool FreeBuffer(SocketAsyncEventArgs value)
        {
            int index = value.Offset;
            
            for(int i = 0; i<_bufferSize; i++)
            {
                _buffer[index + i] = 0;
            }

            _freeBufferPool.Push(index);
            value.SetBuffer(null, 0, 0);

            return true;
        }
    }
}
