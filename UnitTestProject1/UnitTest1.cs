using Microsoft.VisualStudio.TestTools.UnitTesting;
using proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetRequest()
        {
            var tasks = new List<Task>();
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                tasks.Add(Task.Run(() =>
                {
                    if (ip.ToString().Contains(":"))
                        return;

                    if (ip.ToString().Equals("10.94.136.231"))
                    {
                        IPEndPoint localEndPoint = new IPEndPoint(ip, 0);

                        // Create a TCP/IP socket.
                        Socket socket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);
                        socket.Bind(localEndPoint);
                        socket.ReceiveTimeout = 3000;

                        socket.Connect("10.30.138.135", 8090);
                        var requestedUri = new Uri("http://stackoverflow.com/questions/6881569/using-http-proxy-using-socket");

                        string request = string.Empty;
                        string build_request = string.Empty;

                        {
                            request = "GET {0} HTTP/1.1\r\n" +
                                      "Host: {1}\r\n" +
                                      "User-Agent: Mozilla/5.0 (Windows NT 5.1; rv:5.0) Gecko/20100101 Firefox/5.0\r\n" +
                                      "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n" +
                                      "Accept-Language: en-us,en;q=0.5\r\nAccept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7\r\n" +
                                      "Connection: keep-alive\r\n" +
                                      "Referer: {0}\r\n" +
                                      "Cookie: {2}\r\n\r\n";
                            build_request = string.Format(request, requestedUri.AbsoluteUri, requestedUri.Host, "PREF=ID=19495678a6a3dd6e:U=c5ce8e4e3f61da69:FF=0:TM=1311310634:LM=1311310636:S=gbV7hD2dPfycsf8Q; NID=49=dN3QceFFBFxwsCXM43HCRJF_oxoBpUHuUWt2tpoofEDFcRhj7TWWV4EFQNuVYP1GhyBAsQr3oOeohsJp31x8kb_iXiGcQFh1a3IFsPTNKjzJv_NgSK8ssG956PJO7jH-");
                        }

                        byte[] data = Encoding.UTF8.GetBytes(build_request);
                        socket.Send(data, data.Length, 0);

                        int bytes = 0;
                        byte[] bytesReceived = new byte[10240];
                        string currentBatch = string.Empty;
                        StringBuilder responseString = new StringBuilder();

                        try
                        {
                            do
                            {
                                bytes = socket.Receive(bytesReceived);
                                currentBatch = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                                responseString.Append(currentBatch);
                            }
                            while (bytes > 0);
                        }
                        catch (Exception)
                        {
                        }

                        Console.WriteLine("responce from " + ip.ToString());
                        Console.WriteLine(responseString.ToString());
                        socket.Close();
                    }
                    else
                    {
                        IPEndPoint localEndPoint = new IPEndPoint(ip, 0);

                        // Create a TCP/IP socket.
                        Socket socket = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);
                        socket.Bind(localEndPoint);
                        socket.ReceiveTimeout = 3000;

                        socket.Connect("stackoverflow.com", 80);
                        var requestedUri = new Uri("http://stackoverflow.com/questions/6881569/using-http-proxy-using-socket");

                        string request = string.Empty;
                        string build_request = string.Empty;

                        {
                            request = "GET {0} HTTP/1.1\r\n" +
                                      "Host: {1}\r\n" +
                                      "User-Agent: Mozilla/5.0 (Windows NT 5.1; rv:5.0) Gecko/20100101 Firefox/5.0\r\n" +
                                      "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8\r\n" +
                                      "Accept-Language: en-us,en;q=0.5\r\nAccept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.7\r\n" +
                                      "Connection: keep-alive\r\n" +
                                      "Referer: {0}\r\n" +
                                      "Cookie: {2}\r\n\r\n";
                            build_request = string.Format(request, requestedUri.AbsoluteUri, requestedUri.Host, "PREF=ID=19495678a6a3dd6e:U=c5ce8e4e3f61da69:FF=0:TM=1311310634:LM=1311310636:S=gbV7hD2dPfycsf8Q; NID=49=dN3QceFFBFxwsCXM43HCRJF_oxoBpUHuUWt2tpoofEDFcRhj7TWWV4EFQNuVYP1GhyBAsQr3oOeohsJp31x8kb_iXiGcQFh1a3IFsPTNKjzJv_NgSK8ssG956PJO7jH-");
                        }

                        byte[] data = Encoding.UTF8.GetBytes(build_request);
                        socket.Send(data, data.Length, 0);

                        int bytes = 0;
                        byte[] bytesReceived = new byte[10240];
                        string currentBatch = string.Empty;
                        StringBuilder responseString = new StringBuilder();

                        try
                        {
                            do
                            {
                                bytes = socket.Receive(bytesReceived);
                                currentBatch = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                                responseString.Append(currentBatch);
                            }
                            while (bytes > 0);
                        }
                        catch (Exception)
                        {
                        }

                        Console.WriteLine("responce from " + ip.ToString());
                        Console.WriteLine(responseString.ToString());
                        socket.Close();
                    }
                }
                ));
            }
            Task.WaitAll(tasks.ToArray());
        }

        [TestMethod]
        public void socksProxy()
        {
            var tasks = new List<Task>();
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                tasks.Add(Task.Run(() =>
                {
                    if (ip.ToString().Contains(":"))
                        return;

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.de");

                    if (ip.ToString().Equals("10.94.136.231"))
                    {
                        WebProxy myproxy = new WebProxy("10.30.138.135", 8090);
                        myproxy.BypassProxyOnLocal = true;
                        request.Proxy = myproxy;
                    }
                    else
                    {
                        request.Proxy = null;
                    }
                    request.ServicePoint.BindIPEndPointDelegate = delegate (ServicePoint point, IPEndPoint endPoint, int count)
                    {
                        return new IPEndPoint(ip, 0);
                    };
                    request.Method = "GET";
                    Console.WriteLine("Request from: " + ip.ToString());

                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Console.WriteLine("Actual IP: " + response.GetResponseHeader("X-YourIP"));
                    var reader = new StreamReader(response.GetResponseStream());
                    Console.WriteLine("responce from " + ip.ToString() + " " + reader.ReadLine());

                    response.Close();
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }
    }
}