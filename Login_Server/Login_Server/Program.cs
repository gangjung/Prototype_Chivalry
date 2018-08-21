using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Login_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //TcpListener listener = new TcpListener(IPAddress.Any, 8080);
            //listener.Start();

            //byte[] buff = new byte[1024];

            //while (true)
            //{
            //    Console.WriteLine("Connect");

            //    TcpClient tc = listener.AcceptTcpClient();
            //    NetworkStream stream = tc.GetStream();

            //    int a; 

            //    while ((a = stream.Read(buff, 0, buff.Length)) > 0)
            //    {
            //        Console.WriteLine(a);
            //        Console.WriteLine(buff.ToString());
            //        stream.Write(buff, 0, buff.Length);
            //    }
            //    Console.WriteLine(a);

            //    Console.WriteLine("Connect Finish");

            //    stream.Close();
            //    tc.Close();

            Login_Service login_Service = new Login_Service(100);
            login_Service.Start("127.0.0.1", 8080);
            
        }
    }
}
