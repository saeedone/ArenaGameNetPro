using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ArenaGameNet
{
    public class NetworkClient : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private StreamReader reader;
        private bool connected;
        private Thread receiveThread;
        private readonly object sendLock = new object();

        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }

        public event Action<string> OnLog;
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnDataReceived;

        public bool Connect(string ip, int port)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                connected = true;

                ServerIP = ip;
                ServerPort = port;

                receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
                receiveThread.Start();

                OnConnected?.Invoke();
                Log("اتصال برقرار شد: " + ip + ":" + port);
                return true;
            }
            catch (Exception ex)
            {
                Log("خطا در اتصال: " + ex.Message);
                return false;
            }
        }

        public void Disconnect()
        {
            if (!connected) return;

            connected = false;
            try { reader?.Close(); } catch { }
            try { stream?.Close(); } catch { }
            try { client?.Close(); } catch { }
            OnDisconnected?.Invoke();
            Log("اتصال قطع شد");
        }

        public void Send(string data)
        {
            if (!connected || stream == null) return;

            lock (sendLock)
            {
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data + "\n");
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
                catch (Exception ex)
                {
                    Log("خطا در ارسال داده: " + ex.Message);
                }
            }
        }

        private void ReceiveLoop()
        {
            while (connected)
            {
                try
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        Disconnect();
                        break;
                    }
                    OnDataReceived?.Invoke(line);
                }
                catch
                {
                    if (connected) Disconnect();
                    break;
                }
            }
        }

        private void Log(string msg) => OnLog?.Invoke(msg);

        public void Dispose() => Disconnect();
    }
}