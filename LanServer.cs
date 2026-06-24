using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ArenaGameNet
{
    public class LanServer : IDisposable
    {
        private TcpListener listener;
        private bool running;
        private Thread listenThread;
        private ConcurrentDictionary<string, TcpClient> clients = new ConcurrentDictionary<string, TcpClient>();

        public const int GAMENET_PORT = 27016;

        public event Action<string> OnLog;
        public event Action<string> OnPlayerJoined;
        public event Action<string> OnPlayerLeft;

        public string HostIP { get; private set; }
        public int PlayerCount => clients.Count;

        public void Start()
        {
            HostIP = GameNetProtocol.GetLocalIP();
            listener = new TcpListener(IPAddress.Any, GAMENET_PORT);
            listener.Start();
            running = true;

            listenThread = new Thread(AcceptLoop) { IsBackground = true };
            listenThread.Start();

            OnLog?.Invoke($"✓ سرور GameNet روی {HostIP}:{GAMENET_PORT} شروع شد");
        }

        public void Stop()
        {
            running = false;
            try { listener?.Stop(); } catch { }

            foreach (var c in clients.Values)
                try { c.Close(); } catch { }

            clients.Clear();
            OnLog?.Invoke("سرور GameNet متوقف شد");
        }

        private void AcceptLoop()
        {
            while (running)
            {
                try
                {
                    var client = listener.AcceptTcpClient();
                    string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    OnLog?.Invoke("← بازیکن وصل شد: " + clientIP);
                    OnPlayerJoined?.Invoke(clientIP);

                    string id = Guid.NewGuid().ToString("N").Substring(0, 8);
                    clients[id] = client;

                    var thread = new Thread(() => HandleClient(id, client, clientIP)) { IsBackground = true };
                    thread.Start();
                }
                catch
                {
                    if (running) continue;
                    else break;
                }
            }
        }

        private void HandleClient(string id, TcpClient client, string clientIP)
        {
            try
            {
                var stream = client.GetStream();
                var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                var reader = new StreamReader(stream, Encoding.UTF8);

                writer.WriteLine("SERVER_IP:" + HostIP);
                writer.WriteLine("SERVER_PORT:27015");
                writer.WriteLine("STATUS:READY");

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    OnLog?.Invoke($"[{clientIP}] {line}");
                }
            }
            catch { }
            finally
            {
                clients.TryRemove(id, out _);
                OnLog?.Invoke("✗ بازیکن قطع شد: " + clientIP);
                OnPlayerLeft?.Invoke(clientIP);
                try { client.Close(); } catch { }
            }
        }

        public void Broadcast(string message)
        {
            foreach (var c in clients.Values)
            {
                try
                {
                    var writer = new StreamWriter(c.GetStream(), Encoding.UTF8) { AutoFlush = true };
                    writer.WriteLine(message);
                }
                catch { }
            }
        }

        public void Dispose() => Stop();
    }
}