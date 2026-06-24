using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class CreateServerForm : Form
    {
        public ServerConfig Config { get; private set; }
        private bool started = false;

        private TextBox txtName, txtPassword, txtWorkshopCollection, txtKnifeCollection, txtGloveCollection, txtPlayerModelCollection, txtFastDL;
        private ComboBox cboMap, cboMode, cboBotDiff, cboCheats;
        private NumericUpDown nudPlayers, nudBots;
        private Label lblMapPreview;
        private Button btnStart, btnStop, btnTestSkins;
        private Label lblStatus, lblSkinStatus;
        private CheckBox chkWorkshopSkins, chkKnifeSkins, chkGloveSkins, chkPlayerModels, chkAutoDownload;

        private GameLauncher launcher;
        private LanServer lanServer;
        private Settings settings;

        private readonly string[] Maps = {
            "de_dust2","de_mirage","de_inferno","de_nuke","de_overpass",
            "de_ancient","de_anubis","de_vertigo","de_cache","de_train"
        };

        public CreateServerForm(Settings s)
        {
            settings = s;
            launcher = new GameLauncher();
            InitUI();
        }

        private void InitUI()
        {
            Text = "Arena GameNet Pro — Create Server";
            Size = new Size(820, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.TextPri;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            // Header
            var pnlHeader = new Panel { Left = 0, Top = 0, Width = 820, Height = 55, BackColor = Theme.Panel };
            var lblTitle = new Label { Text = "ساخت سرور LAN", Font = new Font("Segoe UI", 13f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 20, Top = 14, AutoSize = true, RightToLeft = RightToLeft.No };
            lblStatus = new Label { Text = "STOPPED", Font = Theme.FontBtn, ForeColor = Theme.Accent, Left = 680, Top = 18, AutoSize = true };
            var btnBack = new FlatButton("← برگشت", Theme.Panel) { Left = 580, Top = 12, Width = 80, Height = 30, Font = Theme.FontSmall };
            btnBack.Click += (s, e) => { StopServer(); Close(); };
            pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblStatus, btnBack });

            int y = 70;

            // === ردیف ۱: نام سرور و مپ ===
            AddLabel("نام سرور:", 20, y);
            txtName = new DarkTextBox { Left = 20, Top = y + 22, Width = 360, Height = 28, Text = "Arena LAN Server", RightToLeft = RightToLeft.No };

            AddLabel("مپ:", 410, y);
            cboMap = new DarkComboBox { Left = 410, Top = y + 22, Width = 380, Height = 28 };
            cboMap.Items.AddRange(Maps);
            cboMap.SelectedIndex = 0;
            cboMap.SelectedIndexChanged += (s, e) => UpdateMapPreview();

            y += 60;

            // === ردیف ۲: تعداد بازیکن، رمز، مود، SV Cheats ===
            AddLabel("تعداد بازیکن:", 20, y);
            nudPlayers = new NumericUpDown { Left = 20, Top = y + 22, Width = 110, Height = 28, Minimum = 2, Maximum = 64, Value = 10, BackColor = Theme.InputBg, ForeColor = Theme.TextPri };

            AddLabel("رمز (اختیاری):", 150, y);
            txtPassword = new DarkTextBox { Left = 150, Top = y + 22, Width = 200, Height = 28, RightToLeft = RightToLeft.No };

            AddLabel("مود بازی:", 380, y);
            cboMode = new DarkComboBox { Left = 380, Top = y + 22, Width = 170, Height = 28 };
            cboMode.Items.AddRange(new object[] { "Casual", "Competitive", "Deathmatch", "Arms Race" });
            cboMode.SelectedIndex = 0;

            AddLabel("SV Cheats:", 570, y);
            cboCheats = new DarkComboBox { Left = 570, Top = y + 22, Width = 150, Height = 28 };
            cboCheats.Items.AddRange(new object[] { "Off", "On" });
            cboCheats.SelectedIndex = 0;

            y += 65;

            // === ردیف ۳: بات ===
            AddLabel("تعداد بات:", 20, y);
            nudBots = new NumericUpDown { Left = 20, Top = y + 22, Width = 100, Height = 28, Minimum = 0, Maximum = 30, Value = 0, BackColor = Theme.InputBg, ForeColor = Theme.TextPri };

            AddLabel("سختی بات:", 140, y);
            cboBotDiff = new DarkComboBox { Left = 140, Top = y + 22, Width = 140, Height = 28 };
            cboBotDiff.Items.AddRange(new object[] { "Easy", "Normal", "Hard", "Expert" });
            cboBotDiff.SelectedIndex = 3;

            y += 65;

            // === بخش اسکین‌ها ===
            chkWorkshopSkins = new CheckBox { Text = "اسکین گان (Workshop)", Left = 20, Top = y, AutoSize = true, ForeColor = Theme.TextPri, Font = Theme.FontSub };
            txtWorkshopCollection = new DarkTextBox { Left = 200, Top = y, Width = 200, Height = 26, PlaceholderText = "Collection ID گان" };

            chkKnifeSkins = new CheckBox { Text = "اسکین نایف", Left = 430, Top = y, AutoSize = true, ForeColor = Theme.TextPri, Font = Theme.FontSub };
            txtKnifeCollection = new DarkTextBox { Left = 560, Top = y, Width = 200, Height = 26, PlaceholderText = "Collection ID نایف" };

            y += 40;

            chkGloveSkins = new CheckBox { Text = "اسکین دستکش", Left = 20, Top = y, AutoSize = true, ForeColor = Theme.TextPri, Font = Theme.FontSub };
            txtGloveCollection = new DarkTextBox { Left = 200, Top = y, Width = 200, Height = 26, PlaceholderText = "Collection ID دستکش" };

            chkPlayerModels = new CheckBox { Text = "لباس و مدل بازیکن (T/CT)", Left = 430, Top = y, AutoSize = true, ForeColor = Theme.TextPri, Font = Theme.FontSub };
            txtPlayerModelCollection = new DarkTextBox { Left = 630, Top = y, Width = 170, Height = 26, PlaceholderText = "Collection ID لباس" };

            y += 45;

            chkAutoDownload = new CheckBox
            {
                Text = "دانلود خودکار اسکین از ورکشاپ",
                Left = 20,
                Top = y,
                AutoSize = true,
                ForeColor = Theme.TextPri,
                Font = Theme.FontSub,
                Checked = true
            };

            AddLabel("FastDL URL:", 280, y);
            txtFastDL = new DarkTextBox { Left = 380, Top = y, Width = 280, Height = 26, PlaceholderText = "http://yourserver.com/skins/" };

            btnTestSkins = new FlatButton("🔍 تست اسکین", Color.FromArgb(70, 70, 100)) { Left = 680, Top = y, Width = 120, Height = 26, Font = Theme.FontSmall };
            btnTestSkins.Click += OnTestSkinsClicked;

            y += 45;

            lblSkinStatus = new Label
            {
                Text = "آماده تست اسکین",
                Left = 20,
                Top = y,
                AutoSize = true,
                ForeColor = Theme.TextSec,
                Font = Theme.FontSmall
            };

            y += 50;

            lblMapPreview = new Label { Left = 20, Top = y, Width = 760, Height = 55, BackColor = Theme.Panel, ForeColor = Theme.TextSec, TextAlign = ContentAlignment.MiddleCenter, Font = Theme.FontSub };
            UpdateMapPreview();

            y += 70;

            btnStart = new FlatButton("▶ شروع سرور", Theme.Green) { Left = 20, Top = y, Width = 200, Height = 44, Font = new Font("Segoe UI", 11f, FontStyle.Bold) };
            btnStart.ForeColor = Color.Black;
            btnStart.Click += OnStartServer;

            btnStop = new FlatButton("⏹ توقف سرور", Theme.Accent) { Left = 240, Top = y, Width = 160, Height = 44, Enabled = false };
            btnStop.Click += OnStopServer;

            var btnConnect = new FlatButton("🎮 اتصال به سرور", Theme.Blue) { Left = 420, Top = y, Width = 160, Height = 44 };
            btnConnect.Click += OnConnectServer;

            Controls.AddRange(new Control[]
            {
                pnlHeader, txtName, cboMap, nudPlayers, txtPassword,
                cboMode, cboCheats, nudBots, cboBotDiff,
                lblMapPreview, btnStart, btnStop, btnConnect,
                chkWorkshopSkins, txtWorkshopCollection,
                chkKnifeSkins, txtKnifeCollection,
                chkGloveSkins, txtGloveCollection,
                chkPlayerModels, txtPlayerModelCollection,
                chkAutoDownload, txtFastDL, btnTestSkins, lblSkinStatus
            });
        }

        private void AddLabel(string text, int x, int y)
        {
            Controls.Add(new Label { Text = text, ForeColor = Theme.TextSec, Font = Theme.FontLabel, Left = x, Top = y, AutoSize = true });
        }

        private void UpdateMapPreview()
        {
            string map = cboMap.SelectedItem?.ToString() ?? "de_dust2";
            lblMapPreview.Text = $"🗺  {map}";
        }

        private void OnStartServer(object s, EventArgs e)
        {
            if (started) return;
            if (string.IsNullOrWhiteSpace(settings.GamePath)) return;

            Config = new ServerConfig
            {
                ServerName = txtName.Text,
                Map = cboMap.SelectedItem.ToString(),
                MaxPlayers = (int)nudPlayers.Value,
                Password = txtPassword.Text,
                GameMode = cboMode.SelectedItem.ToString(),
                BotCount = (int)nudBots.Value,
                BotDifficulty = cboBotDiff.SelectedIndex,
                SvCheats = cboCheats.SelectedIndex == 1,
                UseWorkshopSkins = chkWorkshopSkins.Checked,
                WorkshopCollection = txtWorkshopCollection.Text.Trim(),
                UseKnifeSkins = chkKnifeSkins.Checked,
                KnifeCollection = txtKnifeCollection.Text.Trim(),
                UseGloveSkins = chkGloveSkins.Checked,
                GloveCollection = txtGloveCollection.Text.Trim(),
                UsePlayerModels = chkPlayerModels.Checked,
                PlayerModelCollection = txtPlayerModelCollection.Text.Trim(),
                AutoDownloadSkins = chkAutoDownload.Checked,
                FastDLUrl = txtFastDL.Text.Trim()
            };

            lanServer = new LanServer();
            lanServer.OnLog += msg => { };
            lanServer.Start();

            launcher.LaunchServer(settings.GamePath, Config);

            started = true;
            lblStatus.Text = "RUNNING";
            lblStatus.ForeColor = Theme.Green;
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void OnStopServer(object s, EventArgs e) => StopServer();

        private void StopServer()
        {
            launcher?.Stop();
            lanServer?.Stop();
            started = false;
            if (lblStatus != null) { lblStatus.Text = "STOPPED"; lblStatus.ForeColor = Theme.Accent; }
            if (btnStart != null) btnStart.Enabled = true;
            if (btnStop != null) btnStop.Enabled = false;
        }

        private void OnConnectServer(object s, EventArgs e)
        {
            launcher.LaunchClient(settings.GamePath, "127.0.0.1");
        }

        private void OnTestSkinsClicked(object sender, EventArgs e)
        {
            bool hasGun = chkWorkshopSkins.Checked && !string.IsNullOrWhiteSpace(txtWorkshopCollection.Text);
            bool hasKnife = chkKnifeSkins.Checked && !string.IsNullOrWhiteSpace(txtKnifeCollection.Text);
            bool hasGlove = chkGloveSkins.Checked && !string.IsNullOrWhiteSpace(txtGloveCollection.Text);
            bool hasModel = chkPlayerModels.Checked && !string.IsNullOrWhiteSpace(txtPlayerModelCollection.Text);

            string status = (!hasGun && !hasKnife && !hasGlove && !hasModel)
                ? "❌ هیچ اسکینی انتخاب نشده"
                : "✅ اسکین‌ها آماده تست هستند";

            lblSkinStatus.Text = status;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            StopServer();
            base.OnFormClosing(e);
        }
    }
}