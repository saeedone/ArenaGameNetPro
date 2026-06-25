using System;
using System.Net.Sockets;
using System.Text;

namespace AGP.Services
{
    public class RconService : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _connected;
        private int _reqId = 1;
        private readonly object _lock = new object();

        public bool IsConnected => _connected;
        public string CurrentIP   { get; private set; } = "";
        public int    CurrentPort { get; private set; }

        public bool Connect(string ip, int port, string password)
        {
            Disconnect();
            try
            {
                _client = new TcpClient();
                if (!_client.ConnectAsync(ip, port).Wait(3000)) return false;
                _stream = _client.GetStream();
                _connected = true;
                CurrentIP = ip; CurrentPort = port;
                Send(3, password);
                var r = Read();
                if (r == null || r.Item1 < 0) { Disconnect(); return false; }
                return true;
            }
            catch { Disconnect(); return false; }
        }

        public string Exec(string cmd)
        {
            if (!_connected) return "[RCON not connected]";
            lock (_lock)
            {
                try
                {
                    Send(2, cmd);
                    var r = Read();
                    return r?.Item3?.Trim() ?? "";
                }
                catch { return "[error]"; }
            }
        }

        private void Send(int type, string body)
        {
            byte[] b = Encoding.UTF8.GetBytes(body);
            int size = 4 + 4 + b.Length + 2;
            byte[] pkt = new byte[4 + size];
            Buffer.BlockCopy(BitConverter.GetBytes(size), 0, pkt, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(_reqId++), 0, pkt, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(type), 0, pkt, 8, 4);
            Buffer.BlockCopy(b, 0, pkt, 12, b.Length);
            _stream.Write(pkt, 0, pkt.Length);
        }

        private Tuple<int,int,string> Read()
        {
            try
            {
                byte[] sb = new byte[4];
                _stream.Read(sb, 0, 4);
                int size = BitConverter.ToInt32(sb, 0);
                if (size <= 0 || size > 8192) return null;
                byte[] d = new byte[size];
                int r = 0;
                while (r < size) r += _stream.Read(d, r, size - r);
                return Tuple.Create(BitConverter.ToInt32(d,0), BitConverter.ToInt32(d,4),
                    Encoding.UTF8.GetString(d, 8, Math.Max(0, size - 10)));
            }
            catch { return null; }
        }

        public void Disconnect()
        {
            _connected = false;
            try { _stream?.Close(); } catch { }
            try { _client?.Close(); } catch { }
        }

        public void Dispose() => Disconnect();
    }
}
