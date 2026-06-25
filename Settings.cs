using System;
using System.IO;

namespace AGP
{
    public class Settings
    {
        private static readonly string Dir  = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArenaGameNetPro");
        private static readonly string File = Path.Combine(Dir, "settings.ini");

        public string GamePath   { get; set; } = "";
        public string PlayerName { get; set; } = "Player 1";
        public bool   NoSteam   { get; set; } = true;
        public string LastIP    { get; set; } = "";
        public string RconIP    { get; set; } = "127.0.0.1";
        public int    RconPort  { get; set; } = 27015;
        public string RconPass  { get; set; } = "";

        public static Settings Load()
        {
            var s = new Settings();
            try
            {
                if (!System.IO.File.Exists(File)) return s;
                foreach (var line in System.IO.File.ReadAllLines(File))
                {
                    var idx = line.IndexOf('=');
                    if (idx < 0) continue;
                    var k = line.Substring(0, idx);
                    var v = line.Substring(idx + 1);
                    switch (k)
                    {
                        case "GamePath":   s.GamePath   = v; break;
                        case "PlayerName": s.PlayerName = v; break;
                        case "NoSteam":    s.NoSteam    = v == "1"; break;
                        case "LastIP":     s.LastIP     = v; break;
                        case "RconIP":     s.RconIP     = v; break;
                        case "RconPort":   int.TryParse(v, out s.RconPort); break;
                        case "RconPass":   s.RconPass   = v; break;
                    }
                }
            }
            catch { }
            return s;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                System.IO.File.WriteAllLines(File, new[]
                {
                    "GamePath="   + GamePath,
                    "PlayerName=" + PlayerName,
                    "NoSteam="    + (NoSteam ? "1" : "0"),
                    "LastIP="     + LastIP,
                    "RconIP="     + RconIP,
                    "RconPort="   + RconPort,
                    "RconPass="   + RconPass
                });
                ApplyPlayerName();
            }
            catch { }
        }

        public void ApplyPlayerName()
        {
            if (string.IsNullOrWhiteSpace(GamePath)) return;
            string dir = Path.Combine(GamePath, "game", "bin", "win64", "steam_settings");
            try
            {
                Directory.CreateDirectory(dir);
                System.IO.File.WriteAllText(Path.Combine(dir, "configs.user.ini"),
                    $"[user::general]\r\naccount_name={PlayerName}\r\naccount_steamid=76561198{new Random().Next(100000000, 999999999)}\r\nlanguage=persian\r\n");
            }
            catch { }
        }
    }
}
