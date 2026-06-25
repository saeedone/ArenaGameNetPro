using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class RconTab : UserControl
    {
        private RichTextBox rtbConsole;
        private DarkInput txtIP, txtPort, txtPass, txtCmd;
        private Label lblStatus;

        public RconTab()
        {
            BackColor = Theme.Bg;
            InitUI();
            AppState.OnRconStateChanged += UpdateStatus;
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "RCON Console", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            // پنل اتصال
            var pnlConn = new CardBox(Theme.Blue) { Left=30, Top=72, Width=1100, Height=60 };
            pnlConn.Controls.Add(new FieldLabel("IP") { Left=12, Top=8 });
            txtIP = new DarkInput { Left=12, Top=26, Width=160, Height=28, Text=AppState.Settings.RconIP };
            pnlConn.Controls.Add(new FieldLabel("Port") { Left=185, Top=8 });
            txtPort = new DarkInput { Left=185, Top=26, Width=80, Height=28, Text=AppState.Settings.RconPort.ToString() };
            pnlConn.Controls.Add(new FieldLabel("Password") { Left=278, Top=8 });
            txtPass = new DarkInput { Left=278, Top=26, Width=180, Height=28, PasswordChar='●', Text=AppState.Settings.RconPass };
            var btnConn = new FlatBtn("Connect", Theme.Blue) { Left=470, Top=22, Width=110, Height=30, Font=Theme.Small };
            var btnDisc = new FlatBtn("Disconnect", Theme.Accent) { Left=590, Top=22, Width=120, Height=30, Font=Theme.Small };
            lblStatus = new Label { Text="● Disconnected", ForeColor=Theme.Accent, Font=new Font("Segoe UI",10f,FontStyle.Bold), Left=730, Top=27, AutoSize=true };

            btnConn.Click += (s,e) =>
            {
                if (!int.TryParse(txtPort.Text, out int port)) { AppState.Log("✗ پورت نامعتبر"); return; }
                AppState.Settings.RconIP = txtIP.Text;
                AppState.Settings.RconPort = port;
                AppState.Settings.RconPass = txtPass.Text;
                AppState.Settings.Save();
                AppState.ConnectRcon(txtIP.Text, port, txtPass.Text);
            };
            btnDisc.Click += (s,e) => { AppState.Rcon.Disconnect(); AppState.OnRconStateChanged?.GetType(); UpdateStatus(); AppendLine("قطع شد", Theme.TextSec); };

            pnlConn.Controls.AddRange(new Control[]{ txtIP, txtPort, txtPass, btnConn, btnDisc, lblStatus });

            // کنسول
            rtbConsole = new RichTextBox { Left=30, Top=142, Width=1100, Height=470, BackColor=Theme.LogBg, ForeColor=Theme.Green, Font=Theme.Mono, ReadOnly=true, BorderStyle=BorderStyle.FixedSingle };

            // ارسال دستور
            txtCmd = new DarkInput { Left=30, Top=622, Width=900, Height=40, Font=new Font("Consolas",10f) };
            txtCmd.PlaceholderText = "Type RCON command and press Enter...";
            txtCmd.KeyDown += (s,e) => { if(e.KeyCode==Keys.Enter){ e.SuppressKeyPress=true; SendCmd(); } };
            var btnSend = new FlatBtn("Send", Theme.Blue) { Left=940, Top=622, Width=90, Height=40 };
            var btnClear = new FlatBtn("Clear", Color.FromArgb(40,40,60)) { Left=1040, Top=622, Width=90, Height=40 };
            btnSend.Click += (s,e) => SendCmd();
            btnClear.Click += (s,e) => rtbConsole.Clear();

            Controls.AddRange(new Control[]{ pnlConn, rtbConsole, txtCmd, btnSend, btnClear });
        }

        private void SendCmd()
        {
            string cmd = txtCmd.Text.Trim();
            if (string.IsNullOrEmpty(cmd)) return;
            AppendLine($"► {cmd}", Theme.Gold);
            string resp = AppState.Rcon.Exec(cmd);
            if (!string.IsNullOrWhiteSpace(resp)) AppendLine(resp.Trim(), Theme.TextPri);
            txtCmd.Clear();
        }

        private void UpdateStatus()
        {
            if (InvokeRequired) { Invoke(new Action(UpdateStatus)); return; }
            bool c = AppState.Rcon.IsConnected;
            lblStatus.Text = c ? $"● Connected — {AppState.Rcon.CurrentIP}:{AppState.Rcon.CurrentPort}" : "● Disconnected";
            lblStatus.ForeColor = c ? Theme.Green : Theme.Accent;
        }

        private void AppendLine(string msg, Color color)
        {
            if (rtbConsole.InvokeRequired) { rtbConsole.Invoke(new Action<string,Color>(AppendLine), msg, color); return; }
            rtbConsole.SelectionStart = rtbConsole.TextLength;
            rtbConsole.SelectionColor = color;
            rtbConsole.AppendText($"[{DateTime.Now:HH:mm:ss}]  {msg}\n");
            rtbConsole.ScrollToCaret();
        }
    }
}
