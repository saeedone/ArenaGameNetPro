using System;
using System.Net.Sockets;
using System.Text;

namespace ArenaGameNet.Services
{
    public class RconService
    {
        private TcpClient client;
        private NetworkStream stream;
        private int requestId = 1;

        public bool Connect(string ip, int port, string password)
        {
            try
            {
                client = new TcpClient();
                client.Connect(ip, port);
                stream = client.GetStream();

                SendPacket(requestId++, 3, password);
                var response = ReadPacket();

                return response != null && response.Item1 >= 0;
            }
            catch
            {
                return false;
            }
        }

        public string SendCommand(string command)
        {
            try
            {
                int id = requestId++;
                SendPacket(id, 2, command);
                var response = ReadPacket();
                return response?.Item3 ?? "";
            }
            catch
            {
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
            try { stream?.Close(); client?.Close(); } catch { }
        }
    }
}