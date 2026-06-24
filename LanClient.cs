using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ArenaGameNet
{
    public class LanClient : IDisposable
    {
        private TcpClient client;
        private StreamReader reader;
        private StreamWriter writer;
        private Thread receiveThread;
        private bool connected;

        public event Action<string> OnLog;
        public event Action<string> OnServerInfoReceived;
        public event Action OnDisconnected;

        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }

        public bool Connect(string hostIP)
        {
            try
            {
                client = new TcpClient();
                client.Connect(hostIP, LanServer.GAMENET_PORT);
                var stream = client.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                connected = true;

                receiveThread = new Thread(ReceiveLoop) { IsBackground = true };
                receiveThread.Start();

                OnLog?.Invoke("✓ به سرور GameNet وصل شدیم: " + hostIP);
                writer.WriteLine("HELLO:GameNetPro");
                return true;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("✗ خطا در اتصال: " + ex.Message);
                return false;
            }
        }

        private void ReceiveLoop()
        {
            try
            {
                string line;
                while (connected && (line = reader.ReadLine()) != null)
                {
                    OnLog?.Invoke("← " + line);

                    if (line.StartsWith("SERVER_IP:"))
                        ServerIP = line.Substring(10);
                    else if (line.StartsWith("SERVER_PORT:"))
                        ServerPort = int.Parse(line.Substring(12));
                    else if (line == "STATUS:READY" && ServerIP != null)
                        OnServerInfoReceived?.Invoke(ServerIP + ":" + ServerPort);
                }
            }
            catch { }
            finally
            {
                connected = false;
                OnDisconnected?.Invoke();
            }
        }

        public void Disconnect()
        {
            connected = false;
            try { client?.Close(); } catch { }
        }

        public void Dispose() => Disconnect();
    }
}