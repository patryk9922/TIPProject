using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TIPProjectSerwer
{
    public class Server
    {
        private IPAddress ip;
        private int port;

        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();
        private List<Invitation> invitations = new List<Invitation>();
        private Dictionary<string, string> calls = new Dictionary<string, string>();

        public Server(IPAddress _ip, int _port)
        {
            ip = _ip;
            port = _port;
        }

        public async Task StartServer()
        {
            TcpListener server = new TcpListener(ip, port);

            server.Start();
            Console.WriteLine("Server has started on {0}:{1}, Waiting for a connection...", ip, port);

            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();

                Console.WriteLine("Client connected");

                Task.Run(() => ObslugaKlienta(client));
            }
        }

        private void ObslugaKlienta(TcpClient client)
        {
            bool loggedIn = false;

            Write(client, "100");

            while  (!loggedIn)
            {
                string response = Read(client, 7);

                    string[] data = response.Split("|");

                    if (data[0] == "201")
                    {
                        Write(client, "200");

                        response = Read(client, Convert.ToInt32(data[1]));

                        string[] loginData = response.Split("|");

                        if  (loginData[0] == "110")
                        {
                            if  (Login(loginData[1], loginData[2]))
                            {
                                Console.WriteLine($"User : {loginData[1]} logged in");
                                loggedIn = true;

                                clients.Add(loginData[1], client);

                                Write(client, "300");
                            }
                            else
                            {
                                loggedIn = false;

                                Write(client, "301");
                            }
                        }
                        else if  (loginData[0] == "120")
                        {
                            if (Register(loginData[1], loginData[2]))
                            {
                                Console.WriteLine($"User : {loginData[1]} registered");
                                loggedIn = true;

                                clients.Add(loginData[1], client);
                                Write(client, "310");
                            }
                            else
                            {
                                loggedIn = false;

                                Write(client, "311");
                            }
                        }
                    }
                    else if  (data[0] == "101")
                    {
                        Console.WriteLine("Client disconnected");
                    }
            }

            while  (loggedIn)
            {
                string code = Read(client, 3);

                if (code == "400")
                {
                    CallInitiation(client);
                }
                else if(code == "401")
                {
                    IncomingCall(client);
                }
                else if(code == "405")
                {
                    string user = clients.Where(x => x.Value == client).FirstOrDefault().Key;
                    var remove = calls.Where(x => x.Key == user || x.Value == user).FirstOrDefault();

                    if(remove.Key == user)
                    {
                        Write(clients[remove.Value], "405");
                        Console.WriteLine($"User: {user} ended call with {remove.Value}");
                    }
                    else if(remove.Value == user)
                    {
                        Write(clients[remove.Key], "405");
                        Console.WriteLine($"User: {user} ended call with {remove.Key}");
                    }

                    calls.Remove(remove.Key);
                }
                else if(code == "101")
                {
                    string disconnectedClient = clients.Where(x => x.Value == client).FirstOrDefault().Key;
                    clients.Remove(disconnectedClient);
                    Console.WriteLine($"User: {disconnectedClient} disconnected");
                    loggedIn = false;
                }
            }
        }

        private void CallInitiation(TcpClient client)
        {
            Write(client, "401");

            string response = Read(client, 7);

            string[] data = response.Split("|");

            if (data[0] == "201")
            {
                Write(client, "200");

                string userName = Read(client, Convert.ToInt32(data[1]));

                if (clients.ContainsKey(userName))
                {
                    if (calls.FirstOrDefault(x => x.Key == userName || x.Value == userName).Value == null)
                    {

                        string callerUserName = clients.FirstOrDefault(x => x.Value == client).Key;
                        invitations.Add(
                            new Invitation(callerUserName, client.Client.RemoteEndPoint.ToString(), userName, clients[userName].Client.RemoteEndPoint.ToString()));

                        Invitation iv = invitations.Where(x => x.CallerUsername == callerUserName).FirstOrDefault();

                        Write(clients[userName], "400");

                        while (!iv.UserAccepted) ;

                        if (!iv.UserRejected)
                        {
                            Console.WriteLine($"Call from {callerUserName} to {userName}");
                            calls.Add(callerUserName, userName);

                            Write(client, "401");

                            Write(client, $"201|{iv.CalleeIP.Length}|");

                            Write(client, iv.CalleeIP);
                        }
                        else
                        {
                            Write(client, "402");
                        }

                        invitations.Remove(iv);
                    }
                    else
                    {
                        byte[] error = Encoding.ASCII.GetBytes("403");

                        client.GetStream().Write(error, 0, error.Length);
                    }
                }
                else
                {
                    byte[] error = Encoding.ASCII.GetBytes("404");

                    client.GetStream().Write(error, 0, error.Length);
                }
            }
        }

        private void IncomingCall(TcpClient client)
        {
            Invitation iv = invitations.Where(x => x.CalleeIP == client.Client.RemoteEndPoint.ToString() &&
                    x.CalleeUsername == clients.FirstOrDefault(x => x.Value == client).Key).FirstOrDefault();

            if(iv.IsMe(clients.FirstOrDefault(x => x.Value == client).Key, client.Client.RemoteEndPoint.ToString()))
            {
                string callerUserName = iv.CallerUsername;

                Write(client, $"201|{callerUserName.Length}");

                Write(client, callerUserName);

                string response = Read(client, 3);

                if(response == "401")
                {
                    string callerIP = iv.CallerIP;

                    Write(client, $"201|{callerIP.Length}|");

                    Write(client, callerIP);

                    iv.Accept();
                }
                else if(response == "402")
                {
                    iv.Reject();
                }
            }
        }

        private bool Login(string login, string password)
        {
            FileStream fileStream = new FileStream("TIPDB.bin", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream);
            string[] data = streamReader.ReadToEnd().Split('|');

            foreach (var i in data)
            {
                if (i == login + ";" + password)
                {
                    streamReader.Close();
                    fileStream.Close();
                    return true;
                }
            }

            streamReader.Close();
            fileStream.Close();

            return false;
        }
        
        private bool Register(string login, string password)
        {
            FileStream fileStream = new FileStream("TIPDB.bin", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream);
            string[] data = streamReader.ReadToEnd().Split('|');

            foreach (var i in data)
            {
                if(i.StartsWith(login))
                {
                    streamReader.Close();
                    fileStream.Close();
                    return false;
                }
            }

            streamReader.Close();
            fileStream.Close();

            FileStream fileStream2 = new FileStream("TIPDB.bin", FileMode.Append);
            StreamWriter streamWriter = new StreamWriter(fileStream2);

            string dane = login + ";" + password + "|";
            streamWriter.Write(dane);

            streamWriter.Close();
            fileStream2.Close();

            return true;
        }

        private void Write(TcpClient client, string message)
        {
            byte[] messageB = Encoding.ASCII.GetBytes(message);
            client.GetStream().Write(messageB, 0, messageB.Length);
        }

        private string Read(TcpClient client, int length)
        {
            byte[] messageB = new byte[length];
            client.GetStream().Read(messageB, 0, messageB.Length);
            return Encoding.ASCII.GetString(messageB);
        }
    }
}
