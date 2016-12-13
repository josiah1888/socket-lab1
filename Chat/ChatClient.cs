
/*-----------------------------------------------------------------------
 *
 * Program: ChatClient
 * Purpose: Allow the Server and Client to chat with each other, and create log files
 *          for both of them
 * Usage:   simpleechoclient <compname> [portnum]
 * Authors: Vladimir Georgiev
 *
 *-----------------------------------------------------------------------
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
class SimpleEchoClient
{

    public static void Main(string[] args)
    {
        if ((args.Length < 1) || (args.Length > 2))
        { // Test for correct # of args
            throw new ArgumentException("Parameters: <Server> <Port>");
        }

        IPHostEntry serverInfo = Dns.GetHostEntry(args[0]);//using IPHostEntry support both host name and host IPAddress inputs
        IPAddress[] serverIPaddr = serverInfo.AddressList; //addresslist may contain both IPv4 and IPv6 addresses

        byte[] data = new byte[1024];
        string input, stringData;
        Socket server;
        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            server.Connect(serverIPaddr, Int32.Parse(args[1]));
        }
        catch (SocketException e)
        {
            Console.WriteLine("Unable to connect to server.");
            Console.WriteLine(e.ToString());
            return;
        }

        List<string> log = new List<string>();
        int recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);
        string name;
        Console.WriteLine("Please enter a name");
        name = Console.ReadLine();
        Console.WriteLine("Enter your message");

        while (true)
        {
            input = Console.ReadLine();
            if (input.Length == 0)
                continue;
            if (input == "exit")
                break;
            server.Send(Encoding.ASCII.GetBytes(name + "> " + input));
            data = new byte[1024];
            recv = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            log.Add(stringData);
            Console.WriteLine(stringData);
        }

        System.IO.File.WriteAllLines($@"C:\Users\Public\client{args[1]}.txt", log);
        Console.WriteLine("Disconnecting from server...");
        server.Shutdown(SocketShutdown.Both);
        server.Close();
    }
}