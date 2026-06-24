using System;
using System.Net.Sockets;
using System.Text;

namespace ArenaGameNet
{
    public class RconClient : IDisposable
    {
        private TcpClient client;
        private NetworkStream stream;
        private bool connected;
        private int requestId = 1;

        public event Action<string> OnLog;

        public bool Connect(string ip, int port, string password)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();
                connected = true;

                SendPacket(requestId++, 3, password);
                var resp = ReadPacket();

                if (resp != null && resp.Item1 >= 0)
                {
                    OnLog?.Invoke("✓ RCON متصل شد به " + ip + ":" + port);
                    return true;
                }

                OnLog?.Invoke("✗ رمز RCON اشتباهه");
                return false;
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("✗ خطا در اتصال RCON: " + ex.Message);
                return false;
            }
        }

        public string SendCommand(string cmd)
        {
            if (!connected) return "";
            try
            {
                int id = requestId++;
                SendPacket(id, 2, cmd);
                var resp = ReadPacket();
                return resp?.Item3 ?? "";
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("✗ خطا: " + ex.Message);
                return "";
            }
        }

        private void SendPacket(int id, int type, string body)
        {
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);
            int size = 4 + 4 + bodyBytes.Length + 2;
            byte[] packet = new byte[4 + size];

            Buffer.BlockCopy(BitConverter.GetBytes(size), 0, packet, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(id), 0, packet, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(type), 0, packet, 8, 4);
            Buffer.BlockCopy(bodyBytes, 0, packet, 12, bodyBytes.Length);
            packet[12 + bodyBytes.Length] = 0;
            packet[13 + bodyBytes.Length] = 0;

            stream.Write(packet, 0, packet.Length);
        }

        private Tuple<int, int, string> ReadPacket()
        {
            try
            {
                byte[] sizeBuf = new byte[4];
                stream.Read(sizeBuf, 0, 4);
                int size = BitConverter.ToInt32(sizeBuf, 0);

                byte[] data = new byte[size];
                int read = 0;
                while (read < size)
                    read += stream.Read(data, read, size - read);

                int id = BitConverter.ToInt32(data, 0);
                int type = BitConverter.ToInt32(data, 4);
                string body = Encoding.UTF8.GetString(data, 8, size - 10);

                return Tuple.Create(id, type, body);
            }
            catch { return null; }
        }

        public void Disconnect()
        {
            connected = false;
            try { stream?.Close(); } catch { }
            try { client?.Close(); } catch { }
        }

        public void Dispose() => Disconnect();
    }
}