using System;
using System.Drawing;
using System.Windows.Forms;
using AGP.Services;

namespace AGP.Tabs
{
    public class CreateServerTab : UserControl
    {
        private DarkInput txtName, txtPassword;
        private DarkCombo cboMap, cboMode, cboBotDiff;
        private DarkNumeric nudPlayers, nudBots;
        private FlatBtn btnStart, btnStop, btnConnect;
        private Label lblStatus;
        private RichTextBox rtbLog;
        private bool _started;

        public CreateServerTab()
        {
            BackColor = Theme.Bg;
            InitUI();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Create Server", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });
            lblStatus = new Label { Text = "● STOPPED", Font = new Font("Segoe UI",11f,FontStyle.Bold), ForeColor = Theme.Accent, Left = 950, Top = 25, AutoSize = true };
            Controls.Add(lblStatus);

            // ردیف ۱
            FL("نام سرور",30,75); txtName = new DarkInput { Left=30, Top=95, Width=370, Height=34, Text="Arena LAN Server" }; Controls.Add(txtName);
            FL("مپ",420,75); cboMap = new DarkCombo { Left=420, Top=95, Width=300, Height=34 };
            cboMap.Items.AddRange(new[]{"de_dust2","de_mirage","de_inferno","de_nuke","de_overpass","de_ancient","de_anubis","de_vertigo","de_cache","de_train"});
            cboMap.SelectedIndex=0; Controls.Add(cboMap);
            FL("مود بازی",740,75); cboMode = new DarkCombo { Left=740, Top=95, Width=200, Height=34 };
            cboMode.Items.AddRange(new[]{"Casual","Competitive","Deathmatch","Arms Race"}); cboMode.SelectedIndex=0; Controls.Add(cboMode);

            // ردیف ۲
            FL("تعداد بازیکن",30,148); nudPlayers = new DarkNumeric { Left=30, Top=168, Width=110, Height=34, Minimum=2, Maximum=64, Value=10 }; Controls.Add(nudPlayers);
            FL("تعداد بات",160,148); nudBots = new DarkNumeric { Left=160, Top=168, Width=110, Height=34, Minimum=0, Maximum=30, Value=0 }; Controls.Add(nudBots);
            FL("سختی بات",290,148); cboBotDiff = new DarkCombo { Left=290, Top=168, Width=140, Height=34 };
            cboBotDiff.Items.AddRange(new[]{"Easy","Normal","Hard","Expert"}); cboBotDiff.SelectedIndex=1; Controls.Add(cboBotDiff);
            FL("رمز سرور (اختیاری)",450,148); txtPassword = new DarkInput { Left=450, Top=168, Width=200, Height=34 }; Controls.Add(txtPassword);

            // دکمه‌ها
            btnStart = new FlatBtn("▶  Start Host", Theme.Green) { Left=30, Top=220, Width=200, Height=46, Font=new Font("Segoe UI",12f,FontStyle.Bold) };
            btnStart.ForeColor = Color.Black; btnStart.Click += OnStart; Controls.Add(btnStart);

            btnStop = new FlatBtn("⏹  Stop Server", Theme.Accent) { Left=245, Top=220, Width=180, Height=46, Enabled=false };
            btnStop.Click += OnStop; Controls.Add(btnStop);

            btnConnect = new FlatBtn("🎮  Connect to Server", Theme.Blue) { Left=440, Top=220, Width=200, Height=46 };
            btnConnect.Click += (s,e) =>
            {
                if (string.IsNullOrEmpty(AppState.Settings.GamePath)) { AppState.Log("✗ مسیر بازی تنظیم نشده"); return; }
                AppState.Server.LaunchClient(AppState.Settings.GamePath, "127.0.0.1");
            };
            Controls.Add(btnConnect);

            // اتصال RCON خودکار
            var pnlRcon = new CardBox(Theme.Blue) { Left=30, Top=280, Width=700, Height=62 };
            pnlRcon.Controls.Add(new SectionLabel("RCON AUTO-CONNECT (بعد از روشن شدن سرور)") { Left=12, Top=8 });
            var txtRPass = new DarkInput { Left=12, Top=26, Width=200, Height=28, PlaceholderText="rcon password (default: arenaGNP)" };
            var btnRcon = new FlatBtn("Connect RCON", Theme.Blue) { Left=222, Top=22, Width=150, Height=32, Font=Theme.Small };
            btnRcon.Click += (s,e) =>
            {
                string pass = string.IsNullOrEmpty(txtRPass.Text) ? "arenaGNP" : txtRPass.Text;
                AppState.ConnectRcon("127.0.0.1", 27015, pass);
            };
            pnlRcon.Controls.AddRange(new Control[] { txtRPass, btnRcon });
            Controls.Add(pnlRcon);

            // لاگ
            FL("لاگ سرور:", 30, 355);
            rtbLog = new RichTextBox { Left=30, Top=373, Width=1100, Height=285, BackColor=Theme.LogBg, ForeColor=Theme.Green, Font=Theme.Mono, ReadOnly=true, BorderStyle=BorderStyle.FixedSingle };
            Controls.Add(rtbLog);
        }

        private void OnStart(object s, EventArgs e)
        {
            if (_started) { AppState.Log("سرور قبلاً روشنه"); return; }
            if (string.IsNullOrEmpty(AppState.Settings.GamePath)) { AppState.Log("✗ مسیر بازی در Settings تنظیم نشده"); return; }

            var cfg = new ServerConfig
            {
                Map=cboMap.SelectedItem?.ToString()??"de_dust2",
                GameMode=cboMode.SelectedItem?.ToString()??"Casual",
                MaxPlayers=(int)nudPlayers.Value,
                BotCount=(int)nudBots.Value,
                BotDiff=cboBotDiff.SelectedIndex,
                Password=txtPassword.Text,
                SvCheats=false
            };

            AppState.Server.OnLog += AppendLog;
            AppState.Server.StartServer(AppState.Settings.GamePath, cfg);
            _started = true;
            AppState.SetServerState(true);
            lblStatus.Text="● RUNNING"; lblStatus.ForeColor=Theme.Green;
            btnStart.Enabled=false; btnStop.Enabled=true;
            AppState.Log("✓ سرور شروع شد — برای RCON، چند ثانیه صبر کن بعد Connect RCON رو بزن");
        }

        private void OnStop(object s, EventArgs e)
        {
            AppState.Server.Stop();
            _started=false; AppState.SetServerState(false);
            lblStatus.Text="● STOPPED"; lblStatus.ForeColor=Theme.Accent;
            btnStart.Enabled=true; btnStop.Enabled=false;
        }

        private void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action<string>(AppendLog), msg); return; }
            rtbLog.SelectionColor = msg.Contains("✗")||msg.Contains("ERROR") ? Theme.Accent : Theme.Green;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            rtbLog.ScrollToCaret();
        }

        private void FL(string t, int x, int y) => Controls.Add(new FieldLabel(t) { Left=x, Top=y });
    }
}
