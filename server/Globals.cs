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
    class Globals
    {
        //global variables
        public static int port;
        public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();

        #region Initialization
        //global functions
        //read from config file and initialize
        public static void initialize()
        {
            string configPath = @"../../settings.config";

            if (!File.Exists(configPath))
            {
                string[] lines = {
                                     "#T-Talk settings (Server)",
                                     "",
                                     "# 0 < port_number < 65535",
                                     "port_number=60000"
                                 };
                File.WriteAllLines(configPath, lines);
                Console.WriteLine("Config file created");
                Console.WriteLine("Press enter to terminate");
                Console.ReadLine();
                Environment.Exit(0);
            }

            StreamReader reader = new StreamReader(configPath);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineSplit = line.Split('=');
                if (lineSplit[0] == "port_number")
                    if (checkPort(lineSplit[1]))
                        Globals.port = int.Parse(lineSplit[1]);
                    else
                    {
                        Console.WriteLine("Invalid port number: " + lineSplit[1]);
                        Console.WriteLine("  port number range: " + IPEndPoint.MinPort + " < port number < " + IPEndPoint.MaxPort);
                        Console.WriteLine("\nPress enter to terminate");
                        Console.ReadLine();
                        Environment.Exit(1); //terminate
                    }
                //else if(
                // add more parameters in future
            }
            reader.Close();

            loadRI(); //load room infos
        }

        //checks to see if the port number given in config file is available
        public static bool checkPort(string pn)
        {
            int port = 0;
            if (!int.TryParse(pn, out port)) //returns isInt(pn) and put converted value in port (assigns 0 to port if false)
                return false;

            return port > IPEndPoint.MinPort && port < IPEndPoint.MaxPort;
        }

        //load all the rooms' information
        static void loadRI()
        {
            string riPath = "Data/roomInfo.ri";
            if(!Directory.Exists("Data")) //if it doesn't exists, create one and create rooomInfo.ri
            {
                Directory.CreateDirectory("Data");
                File.Create(riPath);
                return;
            }

            StreamReader reader = new StreamReader(riPath);
            string line = "";
            string rn = "", rt = "", rp = "";
            while ((line = reader.ReadLine()) != null)
            {
                string[] lineSplit = line.Split('=');
                if (lineSplit[0] == "roomName")
                    rn = lineSplit[1];
                else if (lineSplit[0] == "roomType")
                    rt = lineSplit[1];
                else if (lineSplit[0] == "password")
                    rp = lineSplit[1];

                if(line.Length > 0 && line[0] == '-') //add a room
                    Rooms.Add(rn, new Room(rn, rt, rp));
            }
        }
        #endregion

        #region Message Transmission
        public static void send(Socket sock, string m)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] sendBuf = asciiEncoding.GetBytes(m);

            sock.Send(sendBuf);
        }

        public static string receive(Socket sock)
        {
            byte[] message = new byte[100];
            int mSize = sock.Receive(message);

            string m = "";
            for (int i = 0; i < mSize; ++i)
                m += Convert.ToChar(message[i]);

            return m;
        }
        #endregion

        #region Client Dealers
        //create a chat room
        public static void createChat(Socket sock)
        {
        }

        //joins a chat room
        public static void joinChat(Socket sock)
        {
            //begin roomExists
            string rroomName = receive(sock);
            string rroomType = receive(sock);
            bool availability = Rooms.ContainsKey(rroomName) && Rooms[rroomName].roomType == rroomType;
            send(sock, availability.ToString());
            //end roomExists

            if (!availability) //availability sent above (client will handle accordingly)
                return;

            Room foundRoom = Rooms[rroomName]; //get the room object
            if (foundRoom.roomType == "private")
            {
                while (true)
                {
                    string rPassword = receive(sock);
                    bool passwordMatched = rPassword == foundRoom.password;
                    send(sock, passwordMatched.ToString());

                    if (passwordMatched)
                        break;
                    else
                    {
                        string response = receive(sock); //response to retry
                        if (response == "q") //quit
                            return;
                        //else "t": let it iterate in while loop
                    }
                }
                send(sock, "matched");
            }

            Console.WriteLine("\n" + rroomName);
        }

        //
        #endregion
    }
}
