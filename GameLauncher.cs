using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ArenaGameNet
{
    public class GameLauncher
    {
        private Process gameProcess;
        public event Action<string> OnLog;

        public void LaunchServer(string gamePath, ServerConfig cfg)
        {
            string cs2 = Path.Combine(gamePath, "game", "bin", "win64", "cs2.exe");
            if (!File.Exists(cs2))
            {
                OnLog?.Invoke("✗ cs2.exe پیدا نشد");
                return;
            }

            int gameType = 0, gameMode = 0;
            switch (cfg.GameMode)
            {
                case "Competitive": gameType = 0; gameMode = 1; break;
                case "Casual":      gameType = 0; gameMode = 0; break;
                case "Deathmatch":  gameType = 1; gameMode = 2; break;
                case "Arms Race":   gameType = 1; gameMode = 0; break;
                default:            gameType = 0; gameMode = 0; break;
            }

            string botArgs = "";

            if (cfg.BotCount > 0)
            {
                int diff = cfg.BotDifficulty; // 0=Easy, 1=Normal, 2=Hard, 3=Expert

                // تنظیمات حرفه‌ای و واقع‌گرا برای بات‌ها
                botArgs = $" +bot_quota {cfg.BotCount}" +
                          $" +bot_difficulty {diff}" +
                          $" +bot_quota_mode fill" +
                          $" +bot_join_after_player 0" +
                          $" +bot_pathfind 1" +
                          $" +bot_crouch_percent 15" +
                          $" +bot_stop 0" +
                          $" +bot_mimic 0" +
                          $" +bot_autodifficulty_threshold_low -2" +
                          $" +bot_autodifficulty_threshold_high 0.5" +
                          $" +bot_allow_grenades 1" +
                          $" +bot_allow_pistols 1" +
                          $" +bot_allow_submachineguns 1" +
                          $" +bot_allow_rifles 1" +
                          $" +bot_allow_shotguns 1" +
                          $" +bot_allow_machineguns 1";
            }
            else
            {
                botArgs = " +bot_quota 0";
            }

            string passArg = !string.IsNullOrEmpty(cfg.Password) ? $" +sv_password {cfg.Password}" : "";
            string cheats = cfg.SvCheats ? " +sv_cheats 1" : "";

            string skinArgs = "";

            // ===== سیستم حرفه‌ای اسکین (گان + نایف + دستکش + لباس) =====
            if (cfg.AutoDownloadSkins)
            {
                skinArgs += " +sv_allowdownload 1 +sv_allowupload 1 +cl_allowdownload 1 +cl_allowupload 1";

                // گان
                if (cfg.UseWorkshopSkins && !string.IsNullOrEmpty(cfg.WorkshopCollection))
                    skinArgs += $" +host_workshop_collection {cfg.WorkshopCollection}";

                // نایف
                if (cfg.UseKnifeSkins && !string.IsNullOrEmpty(cfg.KnifeCollection))
                    skinArgs += $" +host_workshop_collection {cfg.KnifeCollection}";

                // دستکش (Gloves)
                if (cfg.UseGloveSkins && !string.IsNullOrEmpty(cfg.GloveCollection))
                    skinArgs += $" +host_workshop_collection {cfg.GloveCollection}";

                // مدل بازیکن (T/CT)
                if (cfg.UsePlayerModels && !string.IsNullOrEmpty(cfg.PlayerModelCollection))
                    skinArgs += $" +host_workshop_collection {cfg.PlayerModelCollection}";

                // FastDL
                if (!string.IsNullOrEmpty(cfg.FastDLUrl))
                    skinArgs += $" +sv_downloadurl \"{cfg.FastDLUrl}\"";

                // تنظیمات پیشرفته
                skinArgs += " +sv_forcepreload 1 +cl_forcepreload 1 +net_maxfilesize 256 +sv_downloadqueuesize 4096";
                skinArgs += " +cl_timeout 60 +cl_resend 4 +sv_hibernate_when_empty 0 +sv_precacheinfo 1";
            }
            else
            {
                // فقط بارگذاری بدون دانلود خودکار
                if (cfg.UseWorkshopSkins && !string.IsNullOrEmpty(cfg.WorkshopCollection))
                    skinArgs += $" +host_workshop_collection {cfg.WorkshopCollection}";
                if (cfg.UseKnifeSkins && !string.IsNullOrEmpty(cfg.KnifeCollection))
                    skinArgs += $" +host_workshop_collection {cfg.KnifeCollection}";
                if (cfg.UseGloveSkins && !string.IsNullOrEmpty(cfg.GloveCollection))
                    skinArgs += $" +host_workshop_collection {cfg.GloveCollection}";
                if (cfg.UsePlayerModels && !string.IsNullOrEmpty(cfg.PlayerModelCollection))
                    skinArgs += $" +host_workshop_collection {cfg.PlayerModelCollection}";
            }

            string args = $"-dedicated +map {cfg.Map} +game_type {gameType} +game_mode {gameMode} +sv_lan 1 +port 27015 +maxplayers {cfg.MaxPlayers}{botArgs}{passArg}{cheats}{skinArgs}";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = cs2,
                    Arguments = args,
                    WorkingDirectory = gamePath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    CreateNoWindow = true
                };

                gameProcess = new Process { StartInfo = psi };
                gameProcess.OutputDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) OnLog?.Invoke(e.Data); };
                gameProcess.ErrorDataReceived += (s, e) => { if (!string.IsNullOrEmpty(e.Data)) OnLog?.Invoke(e.Data); };

                gameProcess.Start();
                gameProcess.BeginOutputReadLine();
                gameProcess.BeginErrorReadLine();

                OnLog?.Invoke($"✓ سرور روی {cfg.Map} اجرا شد — پورت 27015");
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("✗ " + ex.Message);
            }
        }

        public void LaunchClient(string gamePath, string serverIP)
        {
            string binPath = Path.Combine(gamePath, "game", "bin", "win64");
            string loader = Path.Combine(binPath, "steamclient_loader_x64.exe");
            string cs2 = Path.Combine(binPath, "cs2.exe");

            string exe = File.Exists(loader) ? loader : cs2;
            string work = File.Exists(loader) ? binPath : gamePath;

            string args = string.IsNullOrEmpty(serverIP) ? "-novid" : $"+connect {serverIP}:27015 -novid";

            try
            {
                gameProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = exe,
                    Arguments = args,
                    WorkingDirectory = work,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal
                });

                OnLog?.Invoke("✓ بازی اجرا شد" + (string.IsNullOrEmpty(serverIP) ? "" : " ← " + serverIP));
            }
            catch (Exception ex)
            {
                OnLog?.Invoke("✗ " + ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                if (gameProcess != null && !gameProcess.HasExited)
                {
                    gameProcess.Kill();
                    OnLog?.Invoke("سرور متوقف شد");
                }
            }
            catch { }
        }

        public bool IsRunning
        {
            get
            {
                try { return gameProcess != null && !gameProcess.HasExited; }
                catch { return false; }
            }
        }
    }
}