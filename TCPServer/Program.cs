using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer
{
    public class TCPServer
    {
        public void SendData(NetworkStream stream, string message)
        {
            byte[] data = Encoding.ASCII.GetBytes(message);
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

                    string response = "I'am get your data, domen is world";
                    SendData(stream, response);
                    Console.WriteLine("*Server: " + response);

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
        }
    }
}
