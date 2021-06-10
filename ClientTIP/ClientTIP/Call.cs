using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientTIP
{
    public class Call
    {
        private UdpClient client;
        private IPEndPoint endPoint;

        public Call(string remoteIp, string localIp)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(remoteIp), 15080);

            client = new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), 15080));

            client.Connect(endPoint);
        }

        public void Close()
        {
            client.Close();
        }

        public void Send()
        {
            byte[] data = Encoding.ASCII.GetBytes("Hello 1");

            int bytesSent = client.Send(data, data.Length);
        }

        public void Receive()
        {
            byte[] data = client.Receive(ref endPoint);

            Console.WriteLine(Encoding.ASCII.GetString(data));
        }
    }
}
