using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCPServer
{
    public class TCPServer
    {
        public void SendData(NetworkStream stream, StringBuilder message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            stream.Write(data, 0, data.Length);
        }

        public string GetData(NetworkStream stream, TcpClient client)
        {
            StringBuilder receive = new StringBuilder();
            while (client.Available != 0)
            {
                byte[] message = new byte[1024];
                int count = stream.Read(message, 0, message.Length);
                receive.Append(Encoding.ASCII.GetString(message, 0, count));
            }

            return receive.ToString();
        }

        private StringBuilder FindInformation(string source)
        {
            StringBuilder inf = new StringBuilder() { };
            using (StreamReader reader = new StreamReader("domens.txt"))
            {
                string line = line = reader.ReadLine().Replace(@"\r\n", ""); ;
                bool isFindDomen = false;
                while (line != null && !isFindDomen)
                {
                    Regex reg = new Regex(@"\r\n");
                    string sourceWithoutSpecSymb = source.TrimEnd('\r', '\n');
                    if (line == sourceWithoutSpecSymb && reg.IsMatch(source))
                    {
                        line = reader.ReadLine();
                        reg = new Regex(@".\\r\\n$");
                        while (line != null && !reg.IsMatch(line))
                        {
                            inf.Append(line + "\n");
                            line = reader.ReadLine();
                        }

                        isFindDomen = true;
                    }
                    line = reader.ReadLine();
                    line = (line != null) ? line.Replace(@"\r\n", "") : line;

                };

                if (!isFindDomen)
                {
                    inf.Append("%This query returned 0 objects.\n\n");
                    inf.Append($"%You queried for {source.TrimEnd('\r', '\n')} but this server does not have any data for {source.TrimEnd('\r', '\n')}\n\n");
                }
                else
                {
                    inf.Insert(0, "%This query returned 1 objects.\n\n");
                }

            }
            return inf;
        }

        public void tcp()
        {
            TcpListener server = null;
            try
            {
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, 43);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Ожидание подключений... ");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Подключен клиент. Выполнение запроса...");
                    NetworkStream stream = client.GetStream();

                    string query = GetData(stream, client);
                    Console.WriteLine("Client: " + query);

                    StringBuilder inf = FindInformation(query);
                    SendData(stream, inf);
                    /*Console.WriteLine("*Server: " + response);*/

                    stream.Close();
                    client.Close();
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server != null)
                    server.Stop();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TCPServer tcp = new TCPServer();
            tcp.tcp();
            Console.ReadLine();
        }
    }
}
