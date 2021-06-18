using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientTIP
{
    public class Call
    {
        private UdpClient client;
        private IPEndPoint endPoint;
        private IWaveIn waveIn;
        private WaveOut waveOut;
        private BufferedWaveProvider bufferedOutProvider;

        static Mutex Lock = new Mutex();
        static LinkedList<byte[]> SenderQueue = new LinkedList<byte[]>();
        static Semaphore SenderKick = new Semaphore(0, int.MaxValue);
        bool loop = false;
        int i = 0;

        public Call(string remoteIp, string localIp)
        {
            try
            {
                endPoint = new IPEndPoint(IPAddress.Parse(remoteIp), 15080);

                client = new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), 15080));

                waveIn = new WaveInEvent();
                waveIn.WaveFormat = new WaveFormat(16000, 16, 1);

                bufferedOutProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1)); //16 - bit PCM, 8KHz sampling, 1 channel
                waveOut = new WaveOut();
                waveOut.Init(bufferedOutProvider);
                waveOut.Play();

                client.Connect(endPoint);
                loop = true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Wave_DataAvailable(object sender, WaveInEventArgs e)
        {
            byte[] buffer = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            Lock.WaitOne();
            SenderQueue.AddLast(buffer);
            Lock.ReleaseMutex();

            SenderKick.Release();
        }

        public void Sender()
        {
            byte[] qbuffer = null;

            while (loop)
            {
                SenderKick.WaitOne();

                Lock.WaitOne();
                bool dataavailable = (SenderQueue.Count != 0);
                if (dataavailable)
                {
                    qbuffer = SenderQueue.First.Value;
                    SenderQueue.RemoveFirst();
                }
                Lock.ReleaseMutex();

                if (!dataavailable) break;

                Lock.WaitOne();
                client.Send(qbuffer, qbuffer.Length);
                Lock.ReleaseMutex();
            }
            i++;
        }

        public void Close()
        {
            while (i < 2) ;
            client.Close();
            client.Dispose();
        }

        public void StartRecording()
        {
            waveIn.StartRecording();
            waveIn.DataAvailable += Wave_DataAvailable;
        }

        public void StopRecording()
        {
            waveIn.StopRecording();
            waveIn.DataAvailable -= Wave_DataAvailable;
            try { waveIn.Dispose(); } catch (Exception) { }
            SenderKick.Release();

            loop = false;
            waveOut.Stop();
            try { waveOut.Dispose(); } catch (Exception) { }
        }

        public void Receiver()
        {
            while (loop)
            {
                try
                {
                    byte[] data = client.Receive(ref endPoint);
                    bufferedOutProvider.AddSamples(data, 0, data.Length);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            i++;
        }
    }
}
