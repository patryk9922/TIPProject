using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ClientTIP
{
    public class SharedClient
    {
        private TcpClient client;

        public SharedClient()
        {
            client = new TcpClient();
        }

        public string GetLocalIP()
        {
            string ip = client.Client.LocalEndPoint.ToString();
            ip = ip.Substring(8);
            ip = ip.Substring(0, ip.IndexOf(']'));
            return ip;
        }

        public bool Connect(string ip, int port)
        {
            byte[] buf = new byte[3];
            try
            {
                client.Connect(IPAddress.Parse(ip), port);

                client.GetStream().Read(buf, 0, buf.Length);

                if(Encoding.ASCII.GetString(buf) == "100")
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        public void Close()
        {
            client.Close();
            client.Dispose();
        }

        public void Write(string message)
        {
            byte[] messageB = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(messageB, 0, messageB.Length);
        }

        public string Read(int length)
        {
            byte[] messageB = new byte[length];
            client.GetStream().Read(messageB, 0, messageB.Length);
            return Encoding.ASCII.GetString(messageB);
        }
    }
}
