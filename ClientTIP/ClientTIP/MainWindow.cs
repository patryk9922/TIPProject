using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ClientTIP
{
    public partial class MainWindow : Form
    {
        private TcpClient client;

        public MainWindow(TcpClient _client)
        {
            InitializeComponent();
            client = _client;

            Task.Run(async () =>
            {
                byte[] buf = new byte[3];

                client.GetStream().Read(buf, 0, buf.Length);

                string code = Encoding.ASCII.GetString(buf);

                if  (code == "400")
                {
                    buf = Encoding.ASCII.GetBytes("200");

                    client.GetStream().Write(buf, 0, buf.Length);

                    byte[] len = new byte[7]; //kod (201) | dlugosc danych

                    client.GetStream().Read(len, 0, len.Length);

                    string[] dlugosc = Encoding.ASCII.GetString(len).Split("|");

                    if (dlugosc[0] == "201")
                    {
                        int dlugoscI = Convert.ToInt32(dlugosc[1]);

                        byte[] data = new byte[dlugoscI];

                        client.GetStream().Read(data, 0, data.Length);

                        string login = Encoding.ASCII.GetString(data);

                        switch  (MessageBox.Show($"Czy chcesz akceptować połączenie od: {login}?", "Połączenie przychodzące", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            case DialogResult.Yes:
                                buf = Encoding.ASCII.GetBytes("401");
                                client.GetStream().Write(buf, 0, buf.Length);

                                client.GetStream().Read(len, 0, len.Length);

                                dlugosc = Encoding.ASCII.GetString(len).Split("|");

                                if  (dlugosc[0] == "201")
                                {
                                    dlugoscI = Convert.ToInt32(dlugosc[1]);
                                    byte[] ipB = new byte[dlugoscI];

                                    buf = Encoding.ASCII.GetBytes("200");

                                    client.GetStream().Write(buf, 0, buf.Length);

                                    client.GetStream().Read(ipB, 0, ipB.Length);

                                    string ip = Encoding.ASCII.GetString(ipB);

                                    MessageBox.Show(ip);
                                }

                                break;
                            case DialogResult.No:
                                buf = Encoding.ASCII.GetBytes("402");
                                client.GetStream().Write(buf, 0, buf.Length);
                                break;
                        }
                    }
                }
            });
        }

        private void CallButton_Click(object sender, EventArgs e)
        {
            byte[] test = Encoding.ASCII.GetBytes("400");
            client.GetStream().Write(test, 0, test.Length);

            byte[] userNameB = Encoding.ASCII.GetBytes(UsernameTextField.Text);
            int len = userNameB.Length;

            byte[] lengthB = Encoding.ASCII.GetBytes($"201|{len}");

            byte[] responseB = new byte[3];
            client.GetStream().Read(responseB, 0, responseB.Length);

            if (Encoding.ASCII.GetString(responseB) == "200")
            {
                client.GetStream().Write(userNameB, 0, userNameB.Length);

                client.GetStream().Read(responseB, 0, responseB.Length);

                string response = Encoding.ASCII.GetString(responseB);

                if(response == "401")
                {
                    byte[] lenB = new byte[7]; //kod (201) | dlugosc danych

                    client.GetStream().Read(lenB, 0, lenB.Length);

                    string[] dlugosc = Encoding.ASCII.GetString(lenB).Split("|");

                    if(dlugosc[0] == "201")
                    {
                        byte[] userDataB = new byte[Convert.ToInt32(dlugosc[1])];

                        client.GetStream().Read(userDataB, 0, userDataB.Length);

                        MessageBox.Show(Encoding.ASCII.GetString(userDataB));
                    }

                }
                else if(response == "402")
                {
                    MessageBox.Show("Użytkownik odrzucił połączenie", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response == "404")
                {
                    MessageBox.Show("Użytkownik jest niedostępny", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
