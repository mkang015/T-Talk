﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace T_TalkC
{
    class Globals
    {
        //global variables
        public static string hostname;
        public static int port;

        //global functions
        //read from config file and initialize
        public static bool initialize()
        {
            string configPath = @"../../settings.config";
            
            if (!File.Exists(configPath))
            {
                string[] lines = {
                                     "#T-Talk settings (Client)",
                                     "",
                                     "# 0 < port_number < 65535",
                                     "port_number=8000",
                                     "",
                                     "#Host name or ip address of the server",
                                     "ip_address="
                                 };
                File.WriteAllLines(configPath, lines);
                Console.WriteLine("Config file created\nPlease set the host's address");
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
                    Globals.hostname = lineSplit[1];
                //else if(
                // add more parameters in future
            }
            reader.Close();
            return true;
        }

        //checks to see if the port number given in config file is available
        public static bool checkPort(string pn)
        {
            int port = 0;
            if (!int.TryParse(pn, out port)) //returns isInt(pn) and put converted value in port (assigns 0 to port if false)
                return false;

            return port > IPEndPoint.MinPort && port < IPEndPoint.MaxPort;
        }
    }
}
