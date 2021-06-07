using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TIPProjectSerwer
{
    public class Server
    {
        private IPAddress ip;
        private int port;

        private Dictionary<string, TcpClient> clients = new Dictionary<string, TcpClient>();

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

            NetworkStream stream = client.GetStream();

            byte[] responseB = Encoding.ASCII.GetBytes("100");

            stream.Write(responseB, 0, responseB.Length);

            while  (!loggedIn)
            {

                    byte[] len = new byte[7]; //kod (201) | dlugosc danych

                    stream.Read(len, 0, len.Length);

                    string[] data = Encoding.ASCII.GetString(len).Split("|");

                    if (data[0] == "201")
                    {
                        int dlugosc = Convert.ToInt32(data[1]);

                        responseB = Encoding.ASCII.GetBytes("200");

                        stream.Write(responseB, 0, responseB.Length);

                        byte[] loginDataB = new byte[dlugosc];

                        stream.Read(loginDataB, 0, loginDataB.Length);

                        string[] loginData = Encoding.ASCII.GetString(loginDataB).Split("|");

                        if  (loginData[0] == "110")
                        {
                            if  (Login(loginData[1], loginData[2]))
                            {
                                Console.WriteLine($"User :{loginData[1]} logged in");
                                loggedIn = true;
                                responseB = Encoding.ASCII.GetBytes("300");

                                clients.Add(loginData[1], client);

                                stream.Write(responseB, 0, responseB.Length);
                            }
                            else
                            {
                                loggedIn = false;
                                responseB = Encoding.ASCII.GetBytes("301");

                                stream.Write(responseB, 0, responseB.Length);
                            }
                        }
                        else if  (loginData[0] == "120")
                        {
                            if (Register(loginData[1], loginData[2]))
                            {
                                Console.WriteLine($"User :{loginData[1]} registered");
                                loggedIn = true;
                                responseB = Encoding.ASCII.GetBytes("310");

                                clients.Add(loginData[1], client);
                                stream.Write(responseB, 0, responseB.Length);
                            }
                            else
                            {
                                loggedIn = false;
                                responseB = Encoding.ASCII.GetBytes("311");

                                stream.Write(responseB, 0, responseB.Length);
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
                byte[] codeB = new byte[3];
                stream.Read(codeB, 0, codeB.Length);
                string code = Encoding.ASCII.GetString(codeB);

                if (code == "400")
                {
                    byte[] len = new byte[7]; //kod (201) | dlugosc danych

                    stream.Read(len, 0, len.Length);

                    string[] data = Encoding.ASCII.GetString(len).Split("|");

                    if (data[0] == "201")
                    {
                        int dlugosc = Convert.ToInt32(data[1]);

                        responseB = Encoding.ASCII.GetBytes("200");

                        stream.Write(responseB, 0, responseB.Length);

                        byte[] userNameB = new byte[dlugosc];

                        stream.Read(userNameB, 0, userNameB.Length);

                        string userName = Encoding.ASCII.GetString(userNameB);

                        if (clients.ContainsKey(userName))
                        {
                            clients[userName].GetStream().Write(codeB);

                            string callerUserName = clients.FirstOrDefault(x => x.Value == client).Key;

                            byte[] callerUserNameB = Encoding.ASCII.GetBytes($"{callerUserName}");

                            int len2 = userNameB.Length;

                            byte[] lengthB = Encoding.ASCII.GetBytes($"201|{len2}");

                            clients[userName].GetStream().Read(responseB, 0, 3);

                            string response = Encoding.ASCII.GetString(responseB);

                            if (response == "200")
                            {
                                clients[userName].GetStream().Write(lengthB, 0, lengthB.Length);

                                clients[userName].GetStream().Write(callerUserNameB, 0, callerUserNameB.Length);

                                clients[userName].GetStream().Read(responseB, 0, 3);
                                string response2 = Encoding.ASCII.GetString(responseB);

                                if (response2 == "401")
                                {
                                    byte[] callerIPB = Encoding.ASCII.GetBytes(client.Client.RemoteEndPoint.ToString());

                                    len2 = callerIPB.Length;

                                    lengthB = Encoding.ASCII.GetBytes($"201|{len2}");

                                    clients[userName].GetStream().Write(lengthB, 0, lengthB.Length);

                                    byte[] responseCodeB = new byte[3];

                                    clients[userName].GetStream().Read(responseCodeB, 0, responseCodeB.Length);

                                    if (Encoding.ASCII.GetString(responseCodeB) == "200")
                                    {
                                        clients[userName].GetStream().Write(callerIPB, 0, callerIPB.Length);

                                        byte[] calleeIPB = Encoding.ASCII.GetBytes(clients[userName].Client.RemoteEndPoint.ToString());

                                        len2 = calleeIPB.Length;

                                        lengthB = Encoding.ASCII.GetBytes($"201|{len2}");

                                        client.GetStream().Write(lengthB, 0, lengthB.Length);

                                        client.GetStream().Read(responseCodeB, 0, responseCodeB.Length);

                                        if (Encoding.ASCII.GetString(responseCodeB) == "200")
                                        {
                                            client.GetStream().Write(calleeIPB, 0, calleeIPB.Length);
                                        }
                                    }
                                }
                                else if (response == "402")
                                {
                                    byte[] denied = Encoding.ASCII.GetBytes("402");

                                    client.GetStream().Write(denied, 0, denied.Length);
                                }
                            }
                        }
                        else
                        {
                            byte[] error = Encoding.ASCII.GetBytes("404");

                            client.GetStream().Write(error, 0, error.Length);
                        }
                    }
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
    }
}
