using System;
using System.IO;

namespace ArenaGameNet
{
    public class Settings
    {
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ArenaGameNetPro");

        private static readonly string SettingsFile = Path.Combine(SettingsDir, "settings.ini");

        public string GamePath { get; set; } = "";
        public string PlayerName { get; set; } = "Player 1";
        public bool NoSteamMode { get; set; } = true;
        public string LastServerIP { get; set; } = "";

        public static Settings Load()
        {
            var s = new Settings();
            try
            {
                if (!File.Exists(SettingsFile)) return s;

                foreach (var line in File.ReadAllLines(SettingsFile))
                {
                    if (line.StartsWith("GamePath=")) s.GamePath = line.Substring(9);
                    else if (line.StartsWith("PlayerName=")) s.PlayerName = line.Substring(11);
                    else if (line.StartsWith("NoSteamMode=")) s.NoSteamMode = line.Substring(12) == "1";
                    else if (line.StartsWith("LastServerIP=")) s.LastServerIP = line.Substring(13);
                }
            }
            catch { }
            return s;
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(SettingsDir);
                File.WriteAllLines(SettingsFile, new[]
                {
                    "GamePath=" + GamePath,
                    "PlayerName=" + PlayerName,
                    "NoSteamMode=" + (NoSteamMode ? "1" : "0"),
                    "LastServerIP=" + LastServerIP
                });
                ApplyPlayerName();
            }
            catch { }
        }

        private void ApplyPlayerName()
        {
            if (string.IsNullOrWhiteSpace(GamePath)) return;

            string settingsDir = Path.Combine(GamePath, "game", "bin", "win64", "steam_settings");
            try
            {
                Directory.CreateDirectory(settingsDir);
                File.WriteAllText(Path.Combine(settingsDir, "configs.user.ini"),
                    $"[user::general]\r\naccount_name={PlayerName}\r\naccount_steamid=7656119800000000{new Random().Next(10, 99)}\r\nlanguage=persian\r\n");
            }
            catch { }
        }
    }
}