using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace T_TalkC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("T-Talk (v.1.0.0) \n");
            //Globals.initialize();

            //string hostName = Dns.GetHostName();
            //Console.WriteLine(hostName);
            TcpClient sock = new TcpClient();
            Console.WriteLine("Connecting...");

            sock.Connect("min", 8000);
            Console.WriteLine("Connected");
            NetworkStream serverStream = default(NetworkStream);



            Console.WriteLine("Press enter to terminate");
            Console.ReadLine();

            //close the socket
            sock.Close();
        }
    }
}
