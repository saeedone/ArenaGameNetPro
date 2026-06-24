using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class RconForm : Form
    {
        private RconClient rcon;
        private RichTextBox rtbHistory;
        private TextBox txtCmd, txtIP, txtPass;
        private Button btnConnect, btnSend;
        private Label lblStatus;
        private Settings settings;

        public RconForm(Settings s)
        {
            settings = s;
            rcon = new RconClient();
            rcon.OnLog += msg => AppendHistory(msg, Theme.TextSec);
            InitUI();
        }

        private void InitUI()
        {
            Text = "Arena GameNet Pro — RCON Panel";
            Size = new Size(920, 700);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.TextPri;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            // Header
            var pnlHeader = new Panel { Left = 0, Top = 0, Width = 920, Height = 55, BackColor = Theme.Panel };
            var lbl = new Label { Text = "RCON PANEL", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 20, Top = 14, AutoSize = true, RightToLeft = RightToLeft.No };
            lblStatus = new Label { Text = "● Disconnected", Font = Theme.FontBtn, ForeColor = Theme.Accent, Left = 680, Top = 18, AutoSize = true };
            var btnBack = new FlatButton("← برگشت", Theme.Panel) { Left = 800, Top = 12, Width = 80, Height = 30, Font = Theme.FontSmall };
            btnBack.Click += (s, e) => Close();
            pnlHeader.Controls.AddRange(new Control[] { lbl, lblStatus, btnBack });

            // Connection Panel
            var pnlConn = new CardPanel(Theme.Accent) { Left = 10, Top = 65, Width = 895, Height = 60 };
            pnlConn.Controls.Add(new LabelSmall("IP سرور") { Left = 10, Top = 5 });
            txtIP = new DarkTextBox { Left = 10, Top = 22, Width = 160, Height = 26, Text = "127.0.0.1", RightToLeft = RightToLeft.No };
            pnlConn.Controls.Add(new LabelSmall("پورت RCON") { Left = 185, Top = 5 });
            var txtPort = new DarkTextBox { Left = 185, Top = 22, Width = 70, Height = 26, Text = "27015", RightToLeft = RightToLeft.No };
            pnlConn.Controls.Add(new LabelSmall("رمز RCON") { Left = 270, Top = 5 });
            txtPass = new DarkTextBox { Left = 270, Top = 22, Width = 160, Height = 26, PasswordChar = '●', RightToLeft = RightToLeft.No };
            btnConnect = new FlatButton("اتصال", Theme.Blue) { Left = 445, Top = 15, Width = 90, Height = 30 };
            btnConnect.Click += (s, e) =>
            {
                if (int.TryParse(txtPort.Text, out int port))
                {
                    bool ok = rcon.Connect(txtIP.Text, port, txtPass.Text);
                    lblStatus.Text = ok ? "● Connected" : "● Failed";
                    lblStatus.ForeColor = ok ? Theme.Green : Theme.Accent;
                }
            };
            pnlConn.Controls.AddRange(new Control[] { txtIP, txtPort, txtPass, btnConnect });

            // Quick Actions
            var pnlQuick = new CardPanel { Left = 10, Top = 135, Width = 895, Height = 175 };
            AddSectionLabel(pnlQuick, "QUICK ACTIONS", 10, 5);

            AddSectionLabel(pnlQuick, "MATCH CONTROLS", 10, 25);
            AddRconBtn(pnlQuick, "Start Match", "mp_warmup_end; mp_restartgame 1", Theme.Green, 10, 42);
            AddRconBtn(pnlQuick, "Restart Round", "mp_restartgame 1", Color.FromArgb(100, 60, 20), 125, 42);
            AddRconBtn(pnlQuick, "Swap Teams", "mp_swapteams", Color.FromArgb(60, 60, 120), 240, 42);
            AddRconBtn(pnlQuick, "Pause Match", "mp_pause_match", Color.FromArgb(80, 80, 30), 355, 42);
            AddRconBtn(pnlQuick, "Unpause", "mp_unpause_match", Color.FromArgb(30, 80, 30), 470, 42);
            AddRconBtn(pnlQuick, "Scramble", "mp_scrambleteams", Color.FromArgb(70, 30, 70), 585, 42);

            AddSectionLabel(pnlQuick, "WARMUP", 10, 88);
            AddRconBtn(pnlQuick, "Start Warmup", "mp_warmup_start", Theme.Blue, 10, 105);
            AddRconBtn(pnlQuick, "Pause Warmup", "mp_warmuptime 999", Color.FromArgb(40, 80, 120), 125, 105);
            AddRconBtn(pnlQuick, "Stop Warmup", "mp_warmup_end", Theme.Accent, 240, 105);

            AddSectionLabel(pnlQuick, "BOTS", 400, 88);
            AddRconBtn(pnlQuick, "Kick All Bots", "bot_kick", Theme.Accent, 400, 105);
            AddRconBtn(pnlQuick, "Add Bot CT", "bot_add ct", Theme.Blue, 515, 105);
            AddRconBtn(pnlQuick, "Add Bot T", "bot_add t", Theme.Gold, 630, 105);

            // Match Settings
            var pnlSettings = new CardPanel { Left = 10, Top = 320, Width = 895, Height = 180 };
            AddSectionLabel(pnlSettings, "MATCH SETTINGS", 10, 5);

            AddSettingField(pnlSettings, "Freeze Time", "mp_freezetime", "5", 10, 25);
            AddSettingField(pnlSettings, "Max Rounds", "mp_maxrounds", "30", 155, 25);
            AddSettingField(pnlSettings, "Warmup Time", "mp_warmuptime", "60", 300, 25);
            AddSettingField(pnlSettings, "Round Time", "mp_roundtime_defuse", "1.92", 445, 25);
            AddSettingField(pnlSettings, "Buy Time", "mp_buytime", "15", 590, 25);
            AddSettingField(pnlSettings, "Start Money", "mp_startmoney", "800", 735, 25);

            AddSettingField(pnlSettings, "Gravity", "sv_gravity", "800", 10, 100);
            AddSettingField(pnlSettings, "Restart Delay", "mp_round_restart_delay", "5", 155, 100);

            AddToggleField(pnlSettings, "AllTalk", "sv_full_alltalk", 300, 100);
            AddToggleField(pnlSettings, "Friendly Fire", "mp_friendlyfire", 445, 100);
            AddToggleField(pnlSettings, "SV Cheats", "sv_cheats", 590, 100);

            // Change Map
            var pnlMap = new CardPanel { Left = 10, Top = 510, Width = 440, Height = 80 };
            AddSectionLabel(pnlMap, "CHANGE MAP", 10, 5);
            var cboMaps = new DarkComboBox { Left = 10, Top = 25, Width = 250 };
            string[] maps = { "de_dust2", "de_mirage", "de_inferno", "de_nuke", "de_overpass", "de_ancient", "de_anubis", "de_vertigo" };
            cboMaps.Items.AddRange(maps);
            cboMaps.SelectedIndex = 0;
            var btnChangeMap = new FlatButton("Change Map", Theme.Blue) { Left = 270, Top = 22, Width = 100, Height = 30 };
            btnChangeMap.Click += (s, e) => SendRcon("changelevel " + cboMaps.SelectedItem);
            pnlMap.Controls.AddRange(new Control[] { cboMaps, btnChangeMap });

            // Send Command
            var pnlCmd = new CardPanel { Left = 460, Top = 510, Width = 445, Height = 80 };
            AddSectionLabel(pnlCmd, "SEND COMMAND", 10, 5);
            txtCmd = new DarkTextBox { Left = 10, Top = 25, Width = 310, Height = 28, RightToLeft = RightToLeft.No };
            txtCmd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SendCurrentCmd(); };
            btnSend = new FlatButton("Send", Theme.Blue) { Left = 328, Top = 22, Width = 80, Height = 32 };
            btnSend.Click += (s, e) => SendCurrentCmd();
            pnlCmd.Controls.AddRange(new Control[] { txtCmd, btnSend });

            // History
            AddSectionLabel(this, "RCON HISTORY", 10, 598);
            rtbHistory = new RichTextBox
            {
                Left = 10, Top = 614, Width = 895, Height = 55,
                BackColor = Theme.LogBg, ForeColor = Theme.Green,
                Font = Theme.FontMono, ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                RightToLeft = RightToLeft.No
            };

            Controls.AddRange(new Control[] { pnlHeader, pnlConn, pnlQuick, pnlSettings, pnlMap, pnlCmd, rtbHistory });
        }

        private void AddSectionLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = Theme.TextDim, Left = x, Top = y, AutoSize = true });
        }

        private void AddRconBtn(Control parent, string label, string cmd, Color color, int x, int y)
        {
            var btn = new FlatButton(label, color) { Left = x, Top = y, Width = 112, Height = 28, Font = Theme.FontSmall };
            btn.Click += (s, e) => SendRcon(cmd);
            parent.Controls.Add(btn);
        }

        private void AddSettingField(Control parent, string label, string cvar, string def, int x, int y)
        {
            parent.Controls.Add(new Label { Text = label, ForeColor = Theme.TextSec, Font = Theme.FontSmall, Left = x, Top = y, AutoSize = true });
            var txt = new DarkTextBox { Left = x, Top = y + 16, Width = 95, Height = 24, Text = def, RightToLeft = RightToLeft.No };
            var btn = new FlatButton("Apply", Color.FromArgb(40, 40, 60)) { Left = x + 98, Top = y + 16, Width = 45, Height = 24, Font = Theme.FontSmall };
            btn.Click += (s, e) => SendRcon($"{cvar} {txt.Text}");
            parent.Controls.Add(txt);
            parent.Controls.Add(btn);
        }

        private void AddToggleField(Control parent, string label, string cvar, int x, int y)
        {
            parent.Controls.Add(new Label { Text = label, ForeColor = Theme.TextSec, Font = Theme.FontSmall, Left = x, Top = y, AutoSize = true });
            var btnOn = new FlatButton("On", Theme.Green) { Left = x, Top = y + 16, Width = 52, Height = 24, Font = Theme.FontSmall };
            var btnOff = new FlatButton("Off", Theme.Accent) { Left = x + 55, Top = y + 16, Width = 52, Height = 24, Font = Theme.FontSmall };
            btnOn.Click += (s, e) => SendRcon($"{cvar} 1");
            btnOff.Click += (s, e) => SendRcon($"{cvar} 0");
            parent.Controls.Add(btnOn);
            parent.Controls.Add(btnOff);
        }

        private void SendCurrentCmd()
        {
            if (!string.IsNullOrWhiteSpace(txtCmd.Text))
            {
                SendRcon(txtCmd.Text);
                txtCmd.Clear();
            }
        }

        private void SendRcon(string cmd)
        {
            string resp = rcon.SendCommand(cmd);
            AppendHistory($"► {cmd}", Theme.Gold);
            if (!string.IsNullOrWhiteSpace(resp))
                AppendHistory(resp.Trim(), Theme.TextPri);
        }

        private void AppendHistory(string msg, Color color)
        {
            if (rtbHistory.InvokeRequired)
            {
                rtbHistory.Invoke(new Action<string, Color>(AppendHistory), msg, color);
                return;
            }
            rtbHistory.SelectionStart = rtbHistory.TextLength;
            rtbHistory.SelectionColor = color;
            rtbHistory.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            rtbHistory.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            rcon?.Dispose();
            base.OnFormClosing(e);
        }
    }
}