using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTIP
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        bool connected = false;

        public Form1()
        {
            InitializeComponent();

            client = new TcpClient();
        }

        private void ConnectClickButton_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                byte[] buf = new byte[3];
                try
                {
                    client.Connect(IPAddress.Parse(IPTextField.Text), 15080);

                    client.GetStream().Read(buf, 0, buf.Length);

                    string message = Encoding.ASCII.GetString(buf);

                    if (message == "100")
                    {
                        connected = true;
                        LoginClickButton.Enabled = true;
                        RegisterClickButton.Enabled = true;
                        ConnectClickButton.Text = "Rozłącz się";
                        label4.Text = "Połączono";
                    }
                    else
                    {
                        throw new Exception();
                    }

                }
                catch
                {
                    MessageBox.Show("Błąd podczas łączenia z serwerem", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                byte[] disconnectB = Encoding.ASCII.GetBytes("101|000");

                client.GetStream().Write(disconnectB, 0, disconnectB.Length);

                connected = false;
                LoginClickButton.Enabled = false;
                RegisterClickButton.Enabled = false;
                ConnectClickButton.Text = "Połącz się";
                label4.Text = "Brak połączenia";
                client.Close();
            }
        }

        private void LoginClickButton_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes($"110|{LoginTextField.Text}|{PasswordTextField.Text}");

            int len = data.Length;

            byte[] code = Encoding.ASCII.GetBytes($"201|{len}");

            client.GetStream().Write(code, 0, code.Length);
            byte[] responseB = new byte[3];
            client.GetStream().Read(responseB, 0, responseB.Length);

            if(Encoding.ASCII.GetString(responseB) == "200")
            {
                client.GetStream().Write(data, 0, data.Length);

                client.GetStream().Read(responseB, 0, responseB.Length);

                string response = Encoding.ASCII.GetString(responseB);

                if(response == "300")
                {
                    MessageBox.Show("Zalogowano", "Logowanie udane", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainWindow mainWindow = new MainWindow(client);

                    mainWindow.Show();
                    this.Hide();
                }
                else if(response == "301")
                {
                    MessageBox.Show("Błędne dane\nSpróbuj ponownie", "Logowanie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RegisterClickButton_Click(object sender, EventArgs e)
        {
            byte[] data = Encoding.ASCII.GetBytes($"120|{LoginTextField.Text}|{PasswordTextField.Text}");

            int len = data.Length;

            byte[] code = Encoding.ASCII.GetBytes($"201|{len}");
            client.GetStream().Write(code, 0, code.Length);
            byte[] responseB = new byte[3];
            client.GetStream().Read(responseB, 0, responseB.Length);

            if(Encoding.ASCII.GetString(responseB) == "200")
            {
                client.GetStream().Write(data, 0, data.Length);
                client.GetStream().Read(responseB, 0, responseB.Length);

                string response = Encoding.ASCII.GetString(responseB);

                if(response == "310")
                {
                    MessageBox.Show("Zarejestrowano pomyślnie", "Rejestracja udana", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainWindow mainWindow = new MainWindow(client);

                    mainWindow.Show();
                    this.Hide();
                }
                else if (response == "311")
                {
                    MessageBox.Show("Użytkownik już istnieje", "Rejestracja nieudana", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Wystąpił błąd", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
