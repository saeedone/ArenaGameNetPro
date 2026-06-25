using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AGP.Services
{
    public class ServerEntry
    {
        public string IP       { get; set; }
        public int    Port     { get; set; }
        public string Name     { get; set; }
        public string Map      { get; set; }
        public int    Players  { get; set; }
        public int    MaxPlayers{ get; set; }
        public bool   HasPass  { get; set; }
    }

    public class LanScanner
    {
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

        public static string GetSubnet()
        {
            string ip = GetLocalIP();
            int last = ip.LastIndexOf('.');
            return last > 0 ? ip.Substring(0, last) : "192.168.1";
        }

        public static bool Ping(string ip, int port, int ms = 300)
        {
            try { using (var t = new TcpClient()) return t.ConnectAsync(ip, port).Wait(ms); }
            catch { return false; }
        }

        public static List<ServerEntry> ScanLAN(int port = 27015, Action<string> progress = null)
        {
            string subnet = GetSubnet();
            var results = new ConcurrentBag<ServerEntry>();

            Parallel.For(1, 255, new ParallelOptions { MaxDegreeOfParallelism = 40 }, i =>
            {
                string ip = subnet + "." + i;
                if (Ping(ip, port))
                {
                    var e = new ServerEntry
                    {
                        IP = ip, Port = port,
                        Name = "Arena LAN Server",
                        Map = "de_dust2",
                        Players = 0, MaxPlayers = 10,
                        HasPass = false
                    };
                    results.Add(e);
                    progress?.Invoke("✓ سرور پیدا شد: " + ip);
                }
            });

            return new List<ServerEntry>(results);
        }
    }
}
