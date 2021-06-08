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
        private SharedClient client;

        public MainWindow(SharedClient _client)
        {
            InitializeComponent();
            client = _client;

            Task.Run(async () =>
            {
                string response = client.Read(3);

                if (response == "400")
                {
                    client.Write("401");

                    response = client.Read(7);

                    string[] dlugosc = response.Split("|");

                    if (dlugosc[0] == "201")
                    {
                        string login = client.Read(Convert.ToInt32(dlugosc[1]));

                        switch (MessageBox.Show($"Czy chcesz akceptować połączenie od: {login}?", "Połączenie przychodzące", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            case DialogResult.Yes:
                                client.Write("401");

                                response = client.Read(7);

                                dlugosc = response.Split("|");

                                if (dlugosc[0] == "201")
                                {
                                    //client.Write("200");
                                    string ip = client.Read(Convert.ToInt32(dlugosc[1]));

                                    MessageBox.Show(ip);
                                }

                                break;
                            case DialogResult.No:
                                client.Write("402");
                                break;
                        }
                    }
                }
                else if (response == "401")
                {
                    string message = UsernameTextField.Text;

                    client.Write($"201|{message.Length}");

                    response = client.Read(3);

                    if (response == "200")
                    {
                        client.Write(message);

                        response = client.Read(3);

                        if (response == "401")
                        {
                            response = client.Read(7);

                            string[] dlugosc = response.Split("|");

                            if (dlugosc[0] == "201")
                            {
                                string userData = client.Read(Convert.ToInt32(dlugosc[1]));

                                MessageBox.Show(userData);
                            }

                        }
                        else if (response == "402")
                        {
                            MessageBox.Show("Użytkownik odrzucił połączenie", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else if (response == "404")
                        {
                            MessageBox.Show("Użytkownik jest niedostępny", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            });
        }

        private void CallButton_Click(object sender, EventArgs e)
        {
            client.Write("400");
        }
    }
}
