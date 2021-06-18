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
        private SharedClient client;
        bool connected = false;

        public Form1()
        {
            InitializeComponent();

            client = new SharedClient();

            IPTextField.Text = "192.168.1.16";

            LoginTextField.Text = "test";
            PasswordTextField.Text = "123";
        }

        private void ConnectClickButton_Click(object sender, EventArgs e)
        {
            if (!connected)
            {
                    if(client.Connect(IPTextField.Text, 15080))
                    {
                        connected = true;
                        LoginClickButton.Enabled = true;
                        RegisterClickButton.Enabled = true;
                        ConnectClickButton.Text = "Rozłącz się";
                        label4.Text = "Połączono";
                    }
                    else
                    {
                        MessageBox.Show("Błąd podczas łączenia z serwerem", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
            }
            else
            {
                client.Write("101|000");

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
            string message = $"110|{LoginTextField.Text}|{PasswordTextField.Text}";

            client.Write($"201|{message.Length}");

            string response = client.Read(3);

            if(response == "200")
            {
                client.Write(message);

                response = client.Read(3);

                if(response == "300")
                {
                    MessageBox.Show("Zalogowano", "Logowanie udane", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainWindow mainWindow = new MainWindow(client);

                    mainWindow.FormClosed += MainWindow_FormClosed;

                    mainWindow.Show();
                    Hide();
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

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Show();
            client.Write("101|000");
            connected = false;
            LoginClickButton.Enabled = false;
            RegisterClickButton.Enabled = false;
            ConnectClickButton.Text = "Połącz się";
            label4.Text = "Brak połączenia";
            client.Close();
        }

        private void RegisterClickButton_Click(object sender, EventArgs e)
        {
            string message = $"120|{LoginTextField.Text}|{PasswordTextField.Text}";

            client.Write($"201|{message.Length}");
            string response = client.Read(3);

            if(response == "200")
            {
                client.Write(message);
                response = client.Read(3);

                if(response == "310")
                {
                    MessageBox.Show("Zarejestrowano", "Rejestracja udana", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    MainWindow mainWindow = new MainWindow(client);

                    mainWindow.FormClosed += MainWindow_FormClosed;

                    mainWindow.Show();
                    Hide();
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
