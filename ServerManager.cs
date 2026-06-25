using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AGP.Services
{
    public class ServerConfig
    {
        public string Map        = "de_dust2";
        public string GameMode   = "Casual";
        public int    MaxPlayers = 10;
        public int    BotCount   = 0;
        public int    BotDiff    = 1;
        public string Password   = "";
        public bool   SvCheats   = false;
    }

    public class ServerManager : IDisposable
    {
        private Process _proc;
        public bool IsRunning => _proc != null && !_proc.HasExited;
        public event Action<string> OnLog;

        public void StartServer(string gamePath, ServerConfig cfg)
        {
            if (string.IsNullOrWhiteSpace(gamePath)) { Log("✗ مسیر بازی تنظیم نشده"); return; }
            string cs2 = Path.Combine(gamePath, "game", "bin", "win64", "cs2.exe");
            if (!File.Exists(cs2)) { Log("✗ cs2.exe پیدا نشد: " + cs2); return; }

            int gt = 0, gm = 0;
            switch (cfg.GameMode)
            {
                case "Competitive": gt = 0; gm = 1; break;
                case "Deathmatch":  gt = 1; gm = 2; break;
                case "Arms Race":   gt = 1; gm = 0; break;
                default:            gt = 0; gm = 0; break;
            }

            string bots  = cfg.BotCount > 0 ? $" +bot_quota {cfg.BotCount} +bot_difficulty {cfg.BotDiff}" : " +bot_quota 0";
            string pass  = !string.IsNullOrEmpty(cfg.Password) ? $" +sv_password {cfg.Password}" : "";
            string cheat = cfg.SvCheats ? " +sv_cheats 1" : "";
            string rcon  = " +rcon_password arenaGNP +sv_rcon_whitelist_address 0.0.0.0";

            string args = $"-dedicated +map {cfg.Map} +game_type {gt} +game_mode {gm} +sv_lan 1 +port 27015 +maxplayers {cfg.MaxPlayers}{bots}{pass}{cheat}{rcon}";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cs2, Arguments = args,
                    WorkingDirectory = gamePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError  = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    CreateNoWindow = true
                };
                _proc = new Process { StartInfo = psi };
                _proc.OutputDataReceived += (s,e) => { if (!string.IsNullOrEmpty(e.Data)) Log(e.Data); };
                _proc.ErrorDataReceived  += (s,e) => { if (!string.IsNullOrEmpty(e.Data)) Log(e.Data); };
                _proc.Start();
                _proc.BeginOutputReadLine();
                _proc.BeginErrorReadLine();
                Log($"✓ سرور روی {cfg.Map} اجرا شد — پورت 27015");
            }
            catch (Exception ex) { Log("✗ " + ex.Message); }
        }

        public void LaunchClient(string gamePath, string ip)
        {
            string bin     = Path.Combine(gamePath, "game", "bin", "win64");
            string loader  = Path.Combine(bin, "steamclient_loader_x64.exe");
            string cs2     = Path.Combine(bin, "cs2.exe");
            string exe     = File.Exists(loader) ? loader : cs2;
            string work    = File.Exists(loader) ? bin : gamePath;
            string args    = string.IsNullOrEmpty(ip) ? "-novid" : $"+connect {ip}:27015 -novid";
            try
            {
                Process.Start(new ProcessStartInfo { FileName = exe, Arguments = args, WorkingDirectory = work, UseShellExecute = true });
                Log("✓ بازی اجرا شد" + (string.IsNullOrEmpty(ip) ? "" : " ← " + ip));
            }
            catch (Exception ex) { Log("✗ " + ex.Message); }
        }

        public void Stop()
        {
            try { if (_proc != null && !_proc.HasExited) { _proc.Kill(); Log("سرور متوقف شد"); } }
            catch { }
        }

        private void Log(string m) => OnLog?.Invoke(m);
        public void Dispose() => Stop();
    }
}
