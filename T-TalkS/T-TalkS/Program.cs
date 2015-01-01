using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace T_TalkS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("T-Talk (v.1.0.0) \n");
            Globals.initialize();

            TcpListener listener = new TcpListener(IPAddress.IPv6Any, Globals.port);
            listener.Start();

            while(true)
            {
                Console.WriteLine("Waiting for connection...");
                Socket sock = listener.AcceptSocket();

                Console.WriteLine("accepted");

                //receive header
                string header = Globals.receive(sock);

                Console.WriteLine(header);
                Console.ReadLine();

                //close the socket
                sock.Close();
            }

            Console.WriteLine("\nPress enter to terminate");
            Console.ReadLine();
        }

        //might not need to do these if IPv6Any works
        /*
        static IPAddress getHostIPAddress()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostName);
            IPAddress[] ipAddress = host.AddressList;

            for (int i = 0; i < ipAddress.Length; ++i)
                Console.WriteLine(ipAddress[i]);

            return ipAddress.
        }
        */
    }
}
