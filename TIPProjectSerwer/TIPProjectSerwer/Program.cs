using System;
using System.Net;

namespace TIPProjectSerwer
{
    class Program
    {
        static string ip = "26.47.44.18";
        static int port = 15080;

        static void Main(string[] args)
        {
            Server server = new Server(IPAddress.Parse(ip), port);

            server.StartServer().Wait();
        }
    }
}
