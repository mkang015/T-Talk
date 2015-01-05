using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace T_TalkC
{
    class Globals
    {
        //global variables
        public static string ip;
        public static int port;
        public static Stream stream;
        public static TcpClient sock = new TcpClient();

        #region Initialization
        //global functions
        //read from config file and initialize
        public static void initialize()
        {
            string configPath = @"../../settings.config";
            
            if (!File.Exists(configPath))
            {
                string[] lines = {
                                     "#T-Talk settings (Client)",
                                     "",
                                     "# 0 < port_number < 65535",
                                     "port_number=60000",
                                     "",
                                     "#Host name or ip address of the server",
                                     "ip_address="
                                 };
                File.WriteAllLines(configPath, lines);
                Console.WriteLine("Config file created\nPlease set the host's address");
                Console.WriteLine("Press Enter to terminate");
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
                else if (lineSplit[0] == "ip_address")
                    Globals.ip = lineSplit[1];
                //else if(
                // add more parameters in future
            }
            reader.Close();
        }

        //checks to see if the port number given in config file is available
        public static bool checkPort(string pn)
        {
            int port = 0;
            if (!int.TryParse(pn, out port)) //returns isInt(pn) and put converted value in port (assigns 0 to port if false)
                return false;

            return port > IPEndPoint.MinPort && port < IPEndPoint.MaxPort;
        }
        #endregion

        #region Startup Menu
        //run startup menu
        public static void startMenu()
        {
            printStartMenu();
            string choice = Console.ReadLine();
            if (isValidChoice(choice))
            {
                choice = choice.ToLower();
                if (choice == "c")
                    createChat();
                else if (choice == "j")
                    joinChat();
                else //quit
                    System.Environment.Exit(0);
            }
            else
                Console.WriteLine("\n Invalid choice. Please try again.");
        }

        //print startup menu
        static void printStartMenu()
        {
            Console.Clear();
            Console.WriteLine("T-Talk (v.1.0.0) \n");
            Console.WriteLine("\nMenu");
            Console.WriteLine("  Create a chat room (c)");
            Console.WriteLine("  Join a chat room (j)");
            Console.WriteLine("  Quit (q)");
        }

        //checks if the choice is valid
        static bool isValidChoice(string c)
        {
            if (c.Length != 1)
                return false;

            c = c.ToLower(); //make it lower
            if (c == "c" || c == "j" || c == "q")
                return true;
            else
                return false;
        }
        #endregion

        #region Chat
        //creates chat room in server with specific room name and password
        // send header message to server "cChat"
        static void createChat()
        {
            send("cChat");

            Console.WriteLine("\nCreate");
            Console.WriteLine("   Chatroom type (public or private): ");
            string c = Console.ReadLine().ToLower();

            Console.WriteLine("\nCreating " + c + " chat");
            Console.WriteLine("   Chatroom name: ");
            string roomName = Console.ReadLine();
            string password = "";

            if (c == "public") //public chat room
            {


            }
            else if (c == "private") //private chat room
            {
                Console.WriteLine("   Chatroom password: ");
                password = Console.ReadLine();

            }
            Console.WriteLine(roomName + " is created and available");
        }

        //tries to join a chat room in server with specific room name and password
        // send header message to server "jChat"
        static void joinChat()
        {
            send("jChat");

            Console.WriteLine("\nJoin");
            Console.WriteLine("   Chatroom type (public or private): ");
            string roomType = Console.ReadLine().ToLower();

            Console.WriteLine("\nJoining a chat room");
            Console.Write("   Chatroom name: ");
            string roomName = Console.ReadLine();
            string password = "";

            if (!roomExists(roomName, roomType)) //communicate with server to check for availability 
            {
                Console.WriteLine("\nA " + roomType + " \"" + roomName + "\" does not exist");
                Console.WriteLine("Press enter to go back to main menu");
                Console.ReadLine();
                return;
            }
            else //room found
            {
                if (roomType == "private")//private chat room
                {
                    while(true)
                    {
                        Console.Write("   Chatroom password: ");
                        password = Console.ReadLine();

                        send(password);
                        bool matched = bool.Parse(receive());
                        if (matched)
                            break;
                        else
                        {
                            Console.WriteLine("\n   Invalid password. quit (q) or try again(t): ");
                            string choice = Console.ReadLine();
                            if (choice.ToLower() == "q")
                                return;
                        }
                    }
                    Console.Clear();
                    loadChatRoom(roomName);
                }
            }
        }

        //communicate with server to check for availability of a room
        // (note: keep track of availability in server side to replace status check message transfer)
        static bool roomExists(string roomName, string roomType)
        {
            send(roomName);
            send(roomType);
            string availability = receive();
            return bool.Parse(availability);
        }

        //loads messages to console
        static void loadChatRoom(string roomName)
        {
            Console.WriteLine("loaded " + roomName);
        }
        #endregion

        #region Message Transmission
        static void send(string m)
        {
            ASCIIEncoding asciiEncoding = new ASCIIEncoding();
            byte[] sendBuf = asciiEncoding.GetBytes(m);

            stream.Write(sendBuf, 0, sendBuf.Length);
        }

        static string receive()
        {
            byte[] message = new byte[100];
            int mSize = stream.Read(message, 0, 100);

            string m = "";
            for (int i = 0; i < mSize; ++i)
                m += Convert.ToChar(message[i]);

            return m;
        }
        #endregion
    }
}
