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

            TcpListener listener = new TcpListener(IPAddress.Any, Globals.port);
            listener.Start();

            while(true)
            {
                Console.WriteLine("Waiting for connection...");
                Socket sock = listener.AcceptSocket();

                Console.WriteLine("accepted");

                //if any sort of socket exception is caught, let go and accept another one.
                try
                {
                    //receive header
                    string header = Globals.receive(sock);

                    if (header == "cChat") //Create a chat room
                        Globals.createChat();
                    else //Join a chat room
                        Globals.joinChat();

                    Console.WriteLine(header);
                    Console.ReadLine();
                }
                catch(SocketException ex)
                {
                    Console.WriteLine("Client disconnected.\n");
                    continue;
                }

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
