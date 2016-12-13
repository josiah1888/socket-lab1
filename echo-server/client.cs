﻿/******************************************************************************
            SimpleEchoClient.cs - Simple Echo client using sockets 

  This program demonstrates the use of Sockets API to connect to an ECHO service, 
  send commands to that service using a socket interface, and receive responses 
  from that service.  The user interface is via a MS Dos window.

  This program has been compiled and tested under Microsoft Visual Studio 2010.

  Copyright 2012 by Ziping Liu for VS2010
  Prepared for CS480, Southeast Missouri State University

******************************************************************************/
/*-----------------------------------------------------------------------
 *
 * Program: SimpleEchoClient
 * Purpose: contact echoserver, send user input and print server response
 * Usage:   simpleechoclient <compname> [portnum]
 * Note:    <compname> can be either a computer name, like localhost, xx.cs.semo.edu
 *          or an IP address, like 150.168.0.1
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

        int recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Console.WriteLine(stringData);
        List<string> log = new List<string>();

        while (true)
        {
            input = Console.ReadLine();
            if (input.Length == 0)
                continue;
            if (input == "exit")
                break;
            server.Send(Encoding.ASCII.GetBytes(input));
            data = new byte[1024];
            recv = server.Receive(data);
            string incomingMessage = Encoding.ASCII.GetString(data, 0, recv);
            log.Add(incomingMessage);
            Console.WriteLine(incomingMessage);
        }

        System.IO.File.WriteAllLines($@"C:\Users\Public\client{args[1]}.txt", log);
        Console.WriteLine("Disconnecting from server...");
        server.Shutdown(SocketShutdown.Both);
        server.Close();
    }
}