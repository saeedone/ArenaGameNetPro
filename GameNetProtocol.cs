using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ArenaGameNet
{
    public static class GameNetProtocol
    {
        public const int DEFAULT_PORT = 27015;

        public static string GetLocalIP()
        {
            try
            {
                using (var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    s.Connect("8.8.8.8", 80);
                    return ((IPEndPoint)s.LocalEndPoint).Address.ToString();
                }
            }
            catch { return "127.0.0.1"; }
        }

        public static bool IsAvailable(string ip, int port, int timeoutMs = 150)
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(timeoutMs);
                    if (success) client.EndConnect(result);
                    return success;
                }
            }
            catch { return false; }
        }

        public static string[] ScanSubnet(string subnet, int port)
        {
            if (string.IsNullOrWhiteSpace(subnet)) return Array.Empty<string>();

            if (subnet.Count(c => c == '.') == 3)
                subnet = subnet.Substring(0, subnet.LastIndexOf('.'));

            string[] parts = subnet.Split('.');
            if (parts.Length != 3) return Array.Empty<string>();

            var results = new ConcurrentBag<string>();

            Parallel.For(1, 255, new ParallelOptions { MaxDegreeOfParallelism = 30 }, i =>
            {
                string ip = $"{subnet}.{i}";
                if (IsAvailable(ip, port))
                    results.Add(ip);
            });

            return results.ToArray();
        }

        public static string GetLocalSubnet()
        {
            string localIP = GetLocalIP();
            int lastDot = localIP.LastIndexOf('.');
            return lastDot < 0 ? "192.168.1" : localIP.Substring(0, lastDot);
        }
    }
}