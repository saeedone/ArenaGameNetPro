using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class DashboardTab : UserControl
    {
        private Label lblServerStatus, lblRconStatus, lblIP, lblPlayers;
        private RichTextBox rtbLog;
        private System.Windows.Forms.Timer tmr;

        public DashboardTab()
        {
            BackColor = Theme.Bg;
            InitUI();
            AppState.OnLog                += msg => AppendLog(msg);
            AppState.OnServerStateChanged += UpdateStatus;
            AppState.OnRconStateChanged   += UpdateStatus;
            AppState.Server.OnLog         += msg => AppState.Log(msg);
            tmr = new System.Windows.Forms.Timer { Interval = 4000 };
            tmr.Tick += (s,e) => UpdateStatus();
            tmr.Start();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Dashboard", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            var cServer = new CardBox(Theme.Accent) { Left = 30, Top = 75, Width = 280, Height = 110 };
            cServer.Controls.Add(new SectionLabel("SERVER STATUS") { Left = 12, Top = 10 });
            lblServerStatus = new Label { Text = "● Stopped", Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = Theme.Accent, Left = 12, Top = 30, AutoSize = true };
            lblPlayers = new Label { Text = "Players: —", Font = Theme.Body, ForeColor = Theme.TextSec, Left = 12, Top = 65, AutoSize = true };
            cServer.Controls.AddRange(new Control[] { lblServerStatus, lblPlayers });

            var cRcon = new CardBox(Theme.Blue) { Left = 330, Top = 75, Width = 280, Height = 110 };
            cRcon.Controls.Add(new SectionLabel("RCON STATUS") { Left = 12, Top = 10 });
            lblRconStatus = new Label { Text = "● Disconnected", Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = Theme.Accent, Left = 12, Top = 30, AutoSize = true };
            lblIP = new Label { Text = "—", Font = Theme.Small, ForeColor = Theme.TextSec, Left = 12, Top = 65, AutoSize = true };
            cRcon.Controls.AddRange(new Control[] { lblRconStatus, lblIP });

            var cMyIP = new CardBox(Theme.Green) { Left = 630, Top = 75, Width = 280, Height = 110 };
            cMyIP.Controls.Add(new SectionLabel("YOUR IP ADDRESS") { Left = 12, Top = 10 });
            var myIP = new Label { Text = Services.LanScanner.GetLocalIP(), Font = new Font("Consolas", 16f, FontStyle.Bold), ForeColor = Theme.Green, Left = 12, Top = 30, AutoSize = true };
            cMyIP.Controls.Add(myIP);

            var cVer = new CardBox(Theme.Purple) { Left = 930, Top = 75, Width = 200, Height = 110 };
            cVer.Controls.Add(new SectionLabel("VERSION") { Left = 12, Top = 10 });
            cVer.Controls.Add(new Label { Text = "v2.0 Pro", Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = Theme.Purple, Left = 12, Top = 30, AutoSize = true });
            cVer.Controls.Add(new Label { Text = "LAN Launcher", Font = Theme.Small, ForeColor = Theme.TextSec, Left = 12, Top = 65, AutoSize = true });

            Controls.Add(new Label { Text = "ACTIVITY LOG", Font = new Font("Segoe UI", 8f, FontStyle.Bold), ForeColor = Theme.TextDim, Left = 30, Top = 200, AutoSize = true });
            rtbLog = new RichTextBox
            {
                Left = 30, Top = 218, Width = 1100, Height = 430,
                BackColor = Theme.LogBg, ForeColor = Theme.Green,
                Font = Theme.Mono, ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            Controls.AddRange(new Control[] { cServer, cRcon, cMyIP, cVer, rtbLog });
        }

        private void UpdateStatus()
        {
            if (InvokeRequired) { Invoke(new Action(UpdateStatus)); return; }
            bool srv = AppState.ServerRunning;
            lblServerStatus.Text = srv ? "● Running" : "● Stopped";
            lblServerStatus.ForeColor = srv ? Theme.Green : Theme.Accent;
            bool rc = AppState.Rcon.IsConnected;
            lblRconStatus.Text = rc ? "● Connected" : "● Disconnected";
            lblRconStatus.ForeColor = rc ? Theme.Green : Theme.Accent;
            lblIP.Text = rc ? AppState.Rcon.CurrentIP + ":" + AppState.Rcon.CurrentPort : "—";
            if (rc) { string r = AppState.Rcon.Exec("status"); if (!string.IsNullOrWhiteSpace(r)) foreach (var l in r.Split('\n')) if (l.Contains("players")) { lblPlayers.Text = l.Trim(); break; } }
        }

        private void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action<string>(AppendLog), msg); return; }
            Color c = Theme.Green;
            if (msg.Contains("✗") || msg.Contains("ERROR")) c = Theme.Accent;
            else if (msg.StartsWith("►")) c = Theme.Gold;
            else if (!msg.StartsWith("✓")) c = Theme.TextSec;
            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionColor = c;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}]  {msg}\n");
            rtbLog.ScrollToCaret();
        }

        protected override void OnHandleDestroyed(EventArgs e) { tmr?.Stop(); base.OnHandleDestroyed(e); }
    }
}
