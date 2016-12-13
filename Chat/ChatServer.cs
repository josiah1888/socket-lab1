
/*-----------------------------------------------------------------------
 *
 * Program: ChatServer
 * Purpose: Allow the Server and Client to chat with each other, and create log files
 *          for both of them
 * Usage:   SimpleEchoServer <portnum>
 * Authors: Vladimir Georgiev
 *-----------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

class SimpleEchoServer
{

    public static void Main(string[] args)
    {
        string input, stringData;
        int recv;
        byte[] data = new byte[1024];

        if (args.Length > 1) // Test for correct # of args
            throw new ArgumentException("Parameters: [<Port>]");

        IPEndPoint ipep = new IPEndPoint(IPAddress.Any, Int32.Parse(args[0]));
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Bind(ipep);
        server.Listen(10);
        List<string> log = new List<string>();

        for (;;)
        {
            Console.WriteLine("Do you need to shut down server? Yes or No");
            string choice = Console.ReadLine();
            if (choice.Contains("Y") || choice.Contains("y"))
            {
                Console.WriteLine("The server is shutting down...");
                break;
            }
            Console.WriteLine("Waiting for a client...");
            Socket client = server.Accept();
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            client.Send(data, data.Length, SocketFlags.None);
            Console.WriteLine("Please enter a name");
            string name = Console.ReadLine();
            data = new byte[1024];
            recv = client.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            log.Add(stringData);
            Console.WriteLine(stringData);
            


            while (true)
            {
                input = Console.ReadLine();
                if (input.Length == 0)
                    continue;
                if (input == "exit")
                    break;
                client.Send(Encoding.ASCII.GetBytes(name + "> " + input));
                data = new byte[1024];
                recv = client.Receive(data);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                log.Add(stringData);
                Console.WriteLine(stringData);
            }
            Console.WriteLine("Disconnected from {0}", clientep.Address);
            client.Close();
        }

        System.IO.File.WriteAllLines($@"C:\Users\Public\Server{args[0]}.txt", log);
        server.Close();
    }
}