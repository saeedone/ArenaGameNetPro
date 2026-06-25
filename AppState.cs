using System;
using AGP.Services;

namespace AGP
{
    // وضعیت مشترک بین همه تب‌ها
    public static class AppState
    {
        public static Settings       Settings      = Settings.Load();
        public static RconService    Rcon          = new RconService();
        public static ServerManager  Server        = new ServerManager();
        public static bool           ServerRunning = false;

        public static event Action<string> OnLog;
        public static event Action         OnServerStateChanged;
        public static event Action         OnRconStateChanged;

        public static void Log(string msg) => OnLog?.Invoke(msg);

        public static void SetServerState(bool running)
        {
            ServerRunning = running;
            OnServerStateChanged?.Invoke();
        }

        public static void ConnectRcon(string ip, int port, string pass)
        {
            bool ok = Rcon.Connect(ip, port, pass);
            Log(ok ? $"✓ RCON متصل شد به {ip}:{port}" : "✗ اتصال RCON ناموفق بود");
            OnRconStateChanged?.Invoke();
        }

        public static string Exec(string cmd)
        {
            string r = Rcon.Exec(cmd);
            Log($"► {cmd}");
            if (!string.IsNullOrWhiteSpace(r)) Log(r);
            return r;
        }
    }
}
