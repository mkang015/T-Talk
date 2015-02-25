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
            Globals.initialize();

            Console.WriteLine("Connecting...\n");

            Globals.sock.Connect(IPAddress.Parse(Globals.ip), Globals.port);
            Console.WriteLine("Client connected");

            //save the stream
            Globals.stream = Globals.sock.GetStream();

            //run start Menu
            while (true)
                Globals.startMenu();



            Console.WriteLine("Press enter to terminate");
            Console.ReadLine();

            //close the socket
            Globals.sock.Close();
        }
    }
}
