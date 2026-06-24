using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public partial class MainForm : Form
    {
        private Settings settings;
        private GameLauncher launcher;
        private LanServer lanServer;
        private LanClient lanClient;

        private RichTextBox rtbLog;
        private Label lblStatus, lblPlayers, lblLocalIP;
        private TextBox txtJoinIP;
        private Button btnJoin, btnScan;
        private FlowLayoutPanel flowServers;

        // کنترل‌های قابل تغییر زبان
        private Label lblBrowserTitle;

        public MainForm()
        {
            settings = Settings.Load();
            launcher = new GameLauncher();
            launcher.OnLog += AppendLog;
            InitUI();
            UpdateLocalIP();
        }

        private void InitUI()
        {
            Text = "Arena GameNet Pro";
            Size = new Size(1000, 680);
            MinimumSize = new Size(1000, 680);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.TextPri;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            // Header
            var pnlHeader = new Panel { Left = 0, Top = 0, Width = 1000, Height = 65, BackColor = Theme.Panel };

            var lblMain = new Label
            {
                Text = "آرنا گیم‌نت پرو",
                Font = new Font("Segoe UI Black", 20f, FontStyle.Bold),
                ForeColor = Color.White,
                Left = 18, Top = 10, AutoSize = true,
                RightToLeft = RightToLeft.No
            };

            var lblSub = new Label
            {
                Text = "arena gamenet pro",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Theme.TextDim,
                Left = 21, Top = 42, AutoSize = true,
                RightToLeft = RightToLeft.No
            };

            var accent = new Panel { Left = 18, Top = 8, Width = 6, Height = 50, BackColor = Theme.Accent };

            lblStatus = new Label { Text = "● آماده", Font = Theme.FontBtn, ForeColor = Theme.Green, Left = 800, Top = 14, AutoSize = true };
            lblPlayers = new Label { Text = "", Font = Theme.FontSub, ForeColor = Theme.TextSec, Left = 800, Top = 36, AutoSize = true };
            lblLocalIP = new Label { Text = "", Font = Theme.FontMono, ForeColor = Theme.TextDim, Left = 800, Top = 50, AutoSize = true };

            var btnNickname = new FlatButton("NickName", Theme.Gold) { Left = 550, Top = 15, Width = 100, Height = 32, Font = Theme.FontSmall };
            btnNickname.Click += (s, e) => ShowNicknameDialog();

            // منوی زبان (ملایم و خوانا)
            var btnLang = new FlatButton("FA / EN", Color.FromArgb(230, 230, 240)) { Left = 460, Top = 15, Width = 80, Height = 32, Font = Theme.FontSmall };
            btnLang.ForeColor = Color.Black;
            btnLang.Click += (s, e) =>
            {
                LanguageManager.IsPersian = !LanguageManager.IsPersian;
                btnLang.Text = LanguageManager.IsPersian ? "FA / EN" : "EN / FA";
                ApplyLanguage();
            };
            pnlHeader.Controls.Add(btnLang);

            var btnRcon = new FlatButton("RCON PANEL", Theme.Blue) { Left = 660, Top = 15, Width = 110, Height = 32, Font = Theme.FontSmall };
            btnRcon.Click += (s, e) => new RconForm(settings).Show();

            var btnStopAll = new FlatButton("Stop Server", Theme.Accent) { Left = 780, Top = 15, Width = 110, Height = 32, Font = Theme.FontSmall };
            btnStopAll.Click += (s, e) => StopAll();

            pnlHeader.Controls.AddRange(new Control[] { accent, lblMain, lblSub, lblStatus, lblPlayers, btnNickname, btnRcon, btnStopAll, btnLang });

            // Left Panel
            var pnlLeft = new Panel { Left = 10, Top = 75, Width = 620, Height = 510, BackColor = Theme.Bg };

            lblBrowserTitle = new Label { Text = LanguageManager.Get("لیست سرورها", "Server Browser"), Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 0, Top = 0, AutoSize = true, RightToLeft = RightToLeft.No };

            var pnlSearch = new Panel { Left = 0, Top = 25, Width = 620, Height = 36, BackColor = Theme.Panel };
            var btnRefresh = new FlatButton("Refresh", Theme.Accent) { Left = 5, Top = 4, Width = 80, Height = 28, Font = Theme.FontSmall };
            btnRefresh.Click += (s, e) => ScanServers();

            var txtSearch = new DarkTextBox { Left = 95, Top = 5, Width = 510, Height = 26, RightToLeft = RightToLeft.No };
            txtSearch.PlaceholderText = "Search by name, map, port...";

            pnlSearch.Controls.AddRange(new Control[] { btnRefresh, txtSearch });

            flowServers = new FlowLayoutPanel
            {
                Left = 0, Top = 65, Width = 620, Height = 440,
                BackColor = Theme.Bg,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            AddEmptyServer();

            pnlLeft.Controls.AddRange(new Control[] { lblBrowserTitle, pnlSearch, flowServers });

            // Right Panel
            var pnlRight = new Panel { Left = 640, Top = 75, Width = 345, Height = 510, BackColor = Theme.Bg };

            var pnlSel = new CardPanel(Theme.Accent) { Left = 0, Top = 0, Width = 345, Height = 100 };
            pnlSel.Controls.Add(new Label { Text = "SELECTED SERVER", Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = Theme.TextDim, Left = 10, Top = 8, AutoSize = true });
            pnlSel.Controls.Add(new Label { Text = "No server selected", Font = Theme.FontSub, ForeColor = Theme.TextSec, Left = 10, Top = 25, AutoSize = true, Name = "lblSelName" });

            var btnConnSel = new FlatButton("CONNECT SERVER", Theme.Green) { Left = 10, Top = 58, Width = 320, Height = 32, Font = Theme.FontBtn };
            btnConnSel.ForeColor = Color.Black;
            btnConnSel.Click += (s, e) => ConnectToSelected();
            pnlSel.Controls.Add(btnConnSel);

            var pnlJoin = new CardPanel { Left = 0, Top = 110, Width = 345, Height = 90 };
            pnlJoin.Controls.Add(new LabelSmall("IP میزبان") { Left = 10, Top = 8 });
            txtJoinIP = new DarkTextBox { Left = 10, Top = 26, Width = 200, Height = 28, RightToLeft = RightToLeft.No };
            txtJoinIP.PlaceholderText = "192.168.1.X";
            if (!string.IsNullOrEmpty(settings.LastServerIP)) txtJoinIP.Text = settings.LastServerIP;

            btnJoin = new FlatButton("وصل شو →", Theme.Blue) { Left = 218, Top = 25, Width = 105, Height = 30 };
            btnJoin.Click += OnJoinClicked;

            btnScan = new FlatButton("🔍 اسکن", Color.FromArgb(100, 70, 0)) { Left = 10, Top = 58, Width = 100, Height = 26, Font = Theme.FontSmall };
            btnScan.Click += OnScanClicked;

            pnlJoin.Controls.AddRange(new Control[] { txtJoinIP, btnJoin, btnScan });

            var pnlCreate = new CardPanel(Theme.Green) { Left = 0, Top = 210, Width = 345, Height = 80 };
            pnlCreate.Controls.Add(new Label { Text = "HOST A GAME", Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 10, Top = 10, AutoSize = true });
            pnlCreate.Controls.Add(new Label { Text = "سرور بساز، بقیه وصل میشن", Font = Theme.FontSub, ForeColor = Theme.TextSec, Left = 10, Top = 32, AutoSize = true });

            var btnCreate = new FlatButton("▶ Create Server", Theme.Green) { Left = 180, Top = 22, Width = 150, Height = 36 };
            btnCreate.ForeColor = Color.Black;
            btnCreate.Click += (s, e) => new CreateServerForm(settings).Show();
            pnlCreate.Controls.Add(btnCreate);

            pnlRight.Controls.Add(new LabelSmall("لاگ") { Left = 0, Top = 300 });
            rtbLog = new RichTextBox
            {
                Left = 0, Top = 318, Width = 345, Height = 190,
                BackColor = Theme.LogBg,
                ForeColor = Theme.Green,
                Font = Theme.FontMono,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                RightToLeft = RightToLeft.No
            };

            pnlRight.Controls.AddRange(new Control[] { pnlSel, pnlJoin, pnlCreate, rtbLog });

            // Footer
            var pnlFooter = new Panel { Left = 0, Top = 593, Width = 1000, Height = 38, BackColor = Theme.Panel };
            var btnCheckUpdate = new FlatButton("Check Update", Color.FromArgb(40, 40, 65)) { Left = 10, Top = 5, Width = 110, Height = 26, Font = Theme.FontSmall };
            var lblVersion = new Label { Text = "Version: 1.0.0", Font = Theme.FontSmall, ForeColor = Theme.TextSec, Left = 130, Top = 10, AutoSize = true, RightToLeft = RightToLeft.No };
            var btnSettings = new FlatButton("Settings", Color.FromArgb(40, 40, 65)) { Left = 800, Top = 5, Width = 80, Height = 26, Font = Theme.FontSmall };
            btnSettings.Click += (s, e) => new SettingsForm(settings).ShowDialog();

            var btnExit = new FlatButton("Exit App", Theme.Accent) { Left = 890, Top = 5, Width = 80, Height = 26, Font = Theme.FontSmall };
            btnExit.Click += (s, e) => { StopAll(); Application.Exit(); };

            pnlFooter.Controls.AddRange(new Control[] { btnCheckUpdate, lblVersion, btnSettings, btnExit });

            Controls.AddRange(new Control[] { pnlHeader, pnlLeft, pnlRight, pnlFooter });
        }

        private void AddEmptyServer()
        {
            flowServers.Controls.Clear();
            var lbl = new Label { Text = "برای پیدا کردن سرور، Refresh بزن", ForeColor = Theme.TextDim, Font = Theme.FontSub, Width = 600, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            flowServers.Controls.Add(lbl);
        }

        private void ScanServers()
        {
            AppendLog("🔍 در حال اسکن شبکه...");
            flowServers.Controls.Clear();
            flowServers.Controls.Add(new Label { Text = "در حال اسکن...", ForeColor = Theme.TextDim, Font = Theme.FontSub, Width = 600, Height = 40, TextAlign = ContentAlignment.MiddleCenter });

            new Thread(() =>
            {
                string subnet = GameNetProtocol.GetLocalSubnet();
                var found = GameNetProtocol.ScanSubnet(subnet, LanServer.GAMENET_PORT);

                this.Invoke(new Action(() =>
                {
                    flowServers.Controls.Clear();
                    if (found.Length == 0)
                    {
                        AppendLog("✗ سروری پیدا نشد");
                        AddEmptyServer();
                    }
                    else
                    {
                        foreach (var ip in found)
                        {
                            AppendLog($"✓ سرور: {ip}");
                            AddServerCard("Arena LAN Server", ip + ":27015", "de_dust2", 0, 10, false);
                        }
                    }
                }));
            }) { IsBackground = true }.Start();
        }

        private void AddServerCard(string name, string ip, string map, int players, int maxPlayers, bool hasPass)
        {
            var card = new Panel { Width = 610, Height = 75, BackColor = Theme.Card, Margin = new Padding(0, 0, 0, 4) };
            card.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Theme.Border), 0, 0, card.Width - 1, card.Height - 1);

            var lblName = new Label { Text = name, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 10, Top = 8, AutoSize = true };
            var lblIP = new Label { Text = ip, Font = Theme.FontMono, ForeColor = Theme.TextSec, Left = 10, Top = 30, AutoSize = true };
            var lblMap = new Label { Text = $"Map: {map}", Font = Theme.FontSmall, ForeColor = Theme.TextSec, Left = 10, Top = 52, AutoSize = true };
            var lblPly = new Label { Text = $"Players: {players}/{maxPlayers}", Font = Theme.FontSmall, ForeColor = Theme.TextSec, Left = 150, Top = 52, AutoSize = true };

            var tagLan = new Label { Text = "LAN", Font = Theme.FontSmall, ForeColor = Theme.Blue, BackColor = Color.FromArgb(20, 50, 90), Left = 400, Top = 50, AutoSize = true, Padding = new Padding(4, 2, 4, 2) };

            var btnSelect = new FlatButton("Select", Theme.Accent) { Left = 510, Top = 22, Width = 80, Height = 30, Font = Theme.FontSmall };
            btnSelect.Click += (s, e) =>
            {
                txtJoinIP.Text = ip.Split(':')[0];
                var lbl = Controls.Find("lblSelName", true);
                if (lbl.Length > 0) lbl[0].Text = $"{name}\n{ip}";
                AppendLog($"✓ سرور انتخاب شد: {name}");
            };

            card.Controls.AddRange(new Control[] { lblName, lblIP, lblMap, lblPly, tagLan, btnSelect });
            flowServers.Controls.Add(card);
        }

        private void ConnectToSelected()
        {
            if (string.IsNullOrWhiteSpace(txtJoinIP.Text)) { AppendLog("✗ اول یه سرور انتخاب کن"); return; }
            if (string.IsNullOrWhiteSpace(settings.GamePath)) { AppendLog("✗ مسیر بازی رو در Settings تنظیم کن"); return; }

            settings.LastServerIP = txtJoinIP.Text;
            settings.Save();
            launcher.LaunchClient(settings.GamePath, txtJoinIP.Text);
        }

        private void OnJoinClicked(object s, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtJoinIP.Text)) { AppendLog("✗ IP وارد کن"); return; }
            if (string.IsNullOrWhiteSpace(settings.GamePath)) { AppendLog("✗ مسیر بازی رو در Settings تنظیم کن"); return; }

            settings.LastServerIP = txtJoinIP.Text;
            settings.Save();

            lanClient = new LanClient();
            lanClient.OnLog += AppendLog;
            lanClient.OnServerInfoReceived += addr =>
            {
                string ip = addr.Split(':')[0];
                launcher.LaunchClient(settings.GamePath, ip);
            };
            lanClient.OnDisconnected += () => SetStatus("● آماده", Theme.Green);

            if (!lanClient.Connect(txtJoinIP.Text))
            {
                AppendLog("GameNet server پیدا نشد — مستقیم وصل میشیم...");
                launcher.LaunchClient(settings.GamePath, txtJoinIP.Text);
            }
        }

        private void OnScanClicked(object s, EventArgs e) => ScanServers();

        private void ShowNicknameDialog()
        {
            var frm = new Form
            {
                Text = "تغییر نام بازیکن",
                Size = new Size(400, 180),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Theme.Bg,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                RightToLeft = RightToLeft.Yes
            };

            frm.Controls.Add(new Label { Text = "نام دلخواه خود را وارد کنید:", ForeColor = Theme.TextPri, Font = Theme.FontSub, Left = 20, Top = 20, AutoSize = true });

            var txt = new DarkTextBox { Left = 20, Top = 45, Width = 340, Height = 28, Text = settings.PlayerName, RightToLeft = RightToLeft.No };

            var btnOk = new FlatButton("ثبت", Theme.Accent) { Left = 270, Top = 90, Width = 90, Height = 32 };
            btnOk.Click += (s2, e2) =>
            {
                settings.PlayerName = txt.Text;
                settings.Save();
                AppendLog($"✓ نام بازیکن تغییر کرد: {settings.PlayerName}");
                frm.Close();
            };

            var btnClose = new FlatButton("بستن", Color.FromArgb(40, 40, 60)) { Left = 165, Top = 90, Width = 90, Height = 32 };
            btnClose.Click += (s2, e2) => frm.Close();

            frm.Controls.AddRange(new Control[] { txt, btnOk, btnClose });
            frm.ShowDialog();
        }

        private void StopAll()
        {
            launcher?.Stop();
            lanServer?.Stop();
            lanClient?.Disconnect();
            SetStatus("● آماده", Theme.Green);
            lblPlayers.Text = "";
        }

        private void UpdateLocalIP()
        {
            lblLocalIP.Text = "IP: " + GameNetProtocol.GetLocalIP();
        }

        private void ApplyLanguage()
        {
            if (lblBrowserTitle != null)
                lblBrowserTitle.Text = LanguageManager.Get("لیست سرورها", "Server Browser");

            // می‌تونی متن‌های بیشتری هم اینجا اضافه کنی
        }

        private void SetStatus(string text, Color color)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => { lblStatus.Text = text; lblStatus.ForeColor = color; }));
            else
            {
                lblStatus.Text = text;
                lblStatus.ForeColor = color;
            }
        }

        public void AppendLog(string msg)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(AppendLog), msg);
                return;
            }

            rtbLog.SelectionStart = rtbLog.TextLength;
            rtbLog.SelectionColor = msg.Contains("✗") || msg.Contains("ERROR") ? Theme.Accent :
                                    msg.Contains("✓") ? Theme.Green : Theme.TextSec;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {msg}\n");
            rtbLog.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopAll();
            base.OnFormClosing(e);
        }
    }
}