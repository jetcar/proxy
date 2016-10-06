﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace proxy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            StartListening();
        }

        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 8090);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            var requestArrays = new List<byte[]>();

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
                requestArrays.Add(state.buffer.Take(bytesRead).ToArray());

                // Check for end-of-file tag. If it is not there, read
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("\r\n\r\n") > -1)
                {
                    if (content.Contains("localhost")
                        || content.Contains("gstatic")
                        || content.Contains("telegram")
                        || content.Contains("visualstudio")
                        )
                    {
                        Send(handler, new byte[0]);
                        return;
                    }

                    // All the data has been read from the
                    // client. Display it on the console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
                    {
                        if (ip.ToString().Equals("10.94.136.231"))
                        {
                            IPEndPoint localEndPoint = new IPEndPoint(ip, 0);

                            // Create a TCP/IP socket.
                            Socket socket = new Socket(AddressFamily.InterNetwork,
                                SocketType.Stream, ProtocolType.Tcp);
                            socket.Bind(localEndPoint);
                            socket.ReceiveTimeout = 3000;

                            socket.Connect("10.30.138.135", 8090);

                            var data = new byte[requestArrays.Sum(x => x.Length)];
                            long index1 = 0;
                            foreach (var listArray in requestArrays)
                            {
                                foreach (var b in listArray)
                                {
                                    data[index1++] = b;
                                }
                            }
                            socket.Send(data, data.Length, 0);

                            int bytes = 0;
                            byte[] bytesReceived = new byte[1000240];
                            var listArrays = new List<byte[]>();

                            try
                            {
                                do
                                {
                                    bytes = socket.Receive(bytesReceived);
                                    if (bytes > 0)
                                        listArrays.Add(bytesReceived.Take(bytes).ToArray());
                                } while (bytes > 0);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                            if (handler.Connected)
                            {
                                var temp = new byte[listArrays.Sum(x => x.Length)];
                                long index = 0;
                                foreach (var listArray in listArrays)
                                {
                                    foreach (var b in listArray)
                                    {
                                        temp[index++] = b;
                                    }
                                }
                                Send(handler, temp);
                            }

                            Console.WriteLine("responce from " + ip.ToString());
                            //Console.WriteLine(responseString.ToString());
                            socket.Close();
                        }
                    }
                }
                else
                {
                    //Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, byte[] byteData)
        {
            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}