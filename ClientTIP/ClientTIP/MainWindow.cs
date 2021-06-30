using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ClientTIP
{
    public partial class MainWindow : Form
    {
        private SharedClient client;
        private bool inCall = false, ok = true;
        private System.Timers.Timer timer;
        private int minutes = 0, seconds = 0;

        public MainWindow(SharedClient _client, string login)
        {
            InitializeComponent();
            client = _client;
            LoginLabel.Text = "Zalogowano jako: " + login;

            timer = new System.Timers.Timer();

            timer.Interval = 1000;
            timer.Elapsed += Timer_Tick;
            timer.Enabled = true;
            timer.Stop();

            Task.Run(async () =>
            {
                try
                {
                    while (ok)
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
                                            string ip = client.Read(Convert.ToInt32(dlugosc[1]));
                                            Task.Run(() =>
                                            {
                                                try
                                                {
                                                    Call call = new Call(ip.Substring(0, ip.IndexOf(':')), client.GetLocalIP());

                                                    call.StartRecording();

                                                    Thread s = new Thread(new ThreadStart(call.Sender));
                                                    Thread r = new Thread(new ThreadStart(call.Receiver));

                                                    s.Start();
                                                    r.Start();

                                                    timer.Start();

                                                    inCall = true;

                                                    Invoke((MethodInvoker)delegate
                                                    {
                                                        EndButton.Enabled = true;
                                                        CallButton.Enabled = false;
                                                    });

                                                    while (inCall)
                                                    {
                                                        int i = 0;
                                                    }

                                                    timer.Stop();

                                                    string minutesS = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
                                                    string secondsS = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
                                                    MessageBox.Show($"Czas połączenia: {minutesS}:{secondsS}", "Połączenie zakończone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                    minutes = 0;
                                                    seconds = 0;

                                                    call.StopRecording();
                                                    call.Close();
                                                }
                                                catch (Exception e)
                                                {
                                                    MessageBox.Show(e.Message);
                                                }
                                            });
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
                                        string ip = client.Read(Convert.ToInt32(dlugosc[1]));

                                        Task.Run(() =>
                                        {
                                            try
                                            {
                                                Call call = new Call(ip.Substring(0, ip.IndexOf(':')), client.GetLocalIP());

                                                call.StartRecording();

                                                Thread s = new Thread(new ThreadStart(call.Sender));
                                                Thread r = new Thread(new ThreadStart(call.Receiver));

                                                s.Start();
                                                r.Start();

                                                timer.Start();

                                                inCall = true;

                                                Invoke((MethodInvoker)delegate
                                                {
                                                    EndButton.Enabled = true;
                                                    CallButton.Enabled = false;
                                                });

                                                while (inCall)
                                                {
                                                    int i = 0;
                                                }

                                                timer.Stop();

                                                string minutesS = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
                                                string secondsS = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();

                                                MessageBox.Show($"Czas połączenia: {minutesS}:{secondsS}", "Połączenie zakończone", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                minutes = 0;
                                                seconds = 0;

                                                Invoke((MethodInvoker)delegate
                                                {
                                                    TimeLabel.Text = $"Czas połączenia:";
                                                });

                                                call.StopRecording();
                                                call.Close();
                                            }
                                            catch (Exception e)
                                            {
                                                MessageBox.Show(e.Message);
                                            }
                                        });
                                    }
                                }
                                else if (response == "402")
                                {
                                    MessageBox.Show("Użytkownik odrzucił połączenie", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Invoke((MethodInvoker)delegate
                                    {
                                        LogoutButton.Enabled = true;
                                    });
                                }
                                else if (response == "403")
                                {
                                    MessageBox.Show("Użytkownik aktualnie rozmawia", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Invoke((MethodInvoker)delegate
                                    {
                                        LogoutButton.Enabled = true;
                                    });
                                }
                                else if (response == "404")
                                {
                                    MessageBox.Show("Użytkownik jest niedostępny", "Połączenie nieudane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    Invoke((MethodInvoker)delegate
                                    {
                                        LogoutButton.Enabled = true;
                                    });
                                }
                            }
                        }
                        else if (response == "405")
                        {
                            inCall = false;
                            Invoke((MethodInvoker)delegate
                            {
                                EndButton.Enabled = false;
                                LogoutButton.Enabled = true;
                            });
                        }
                    }
                }
                catch
                {
                }
            });
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(seconds > 58)
            {
                seconds = 0;
                minutes++;
            }
            else
            {
                seconds++;
            }

            Invoke((MethodInvoker)delegate
            {
                string minutesS = minutes < 10 ? "0" + minutes.ToString() : minutes.ToString();
                string secondsS = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
                TimeLabel.Text = $"Czas połączenia: {minutesS}:{secondsS}";
            });
        }

        private void CallButton_Click(object sender, EventArgs e)
        {
            client.Write("400");
            LogoutButton.Enabled = false;
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            ok = false;
            Close();
        }

        private void EndButton_Click(object sender, EventArgs e)
        {
            client.Write("405");
            inCall = false;
            EndButton.Enabled = false;
            LogoutButton.Enabled = true;
        }
    }
}
