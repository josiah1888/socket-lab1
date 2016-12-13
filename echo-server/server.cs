/******************************************************************************
             SimpleEchoServer.cs - Simple TCP echo server using sockets

  This program demonstrates the use of socket APIs to echo back the
  client sentence.  The user interface is via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2010.

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleEchoServer
 * Purpose: wait for a connection from an echo client and echo data
 * Usage:   SimpleEchoServer <portnum>
 *
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

            while (true)
            {
                data = new byte[1024];
                recv = client.Receive(data);
                if (recv == 0)
                    break;
                string incomingMessage = Encoding.ASCII.GetString(data, 0, recv);
                log.Add(incomingMessage);
                Console.WriteLine(incomingMessage);
                client.Send(data, recv, SocketFlags.None);
            }
            Console.WriteLine("Disconnected from {0}", clientep.Address);
            client.Close();
        }

        System.IO.File.WriteAllLines($@"C:\Users\Public\server{args[0]}.txt", log);
        server.Close();
    }
}