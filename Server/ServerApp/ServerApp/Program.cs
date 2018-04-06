using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp
{
    public class Server
    {
        private static TcpListener tcpListener;
        private static List<TcpClient> tcpClientsList = new List<TcpClient>();

        static void Main(string[] args)
        {
            int port;
            Console.WriteLine("Input Port : ");
            port = Convert.ToInt32(Console.ReadLine());
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();

            Console.WriteLine("Server Port : " + port.ToString() + "/n" + "Server started");

            while (true)
            {
                TcpClient tcpClient = tcpListener.AcceptTcpClient();
                tcpClientsList.Add(tcpClient);

                Thread thread = new Thread(ClientListener);
                thread.Start(tcpClient);
            }
        }

        public static void ClientListener(object obj)
        {
            TcpClient tcpClient = (TcpClient)obj;
            //StreamReader reader = new StreamReader(tcpClient.GetStream());
            NetworkStream reader = tcpClient.GetStream();

            Console.WriteLine("Client connected");

            while (true)
            {
                try
                {
                    reader.Flush();
                    byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                    int byteread = reader.Read(buffer, 0, tcpClient.ReceiveBufferSize);
                    string message = Encoding.ASCII.GetString(buffer, 0, byteread);
                    //string message = reader.ReadLine();
                    BroadCast(message, tcpClient);
                    Console.WriteLine(message);
                    reader.Flush();
                }
                
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    reader.Close();
                    reader.Dispose();
                    tcpClient.Close();
                    tcpClient.Dispose();
                    break;
                }
            }
        }

        public static void BroadCast(string msg, TcpClient Client)
        {
            foreach (TcpClient client in tcpClientsList)
            {
                if (client != Client)
                {
                    StreamWriter sWriter = new StreamWriter(client.GetStream());
                    sWriter.WriteLine(msg);
                    sWriter.Flush();
                }
            }
        }
    }
}
