using System;
using System.Drawing;
using System.Windows.Forms;
using AGP.Tabs;

namespace AGP
{
    public class MainForm : Form
    {
        private Panel pnlHeader, pnlNav, pnlContent;
        private Label lblStatus, lblRcon, lblIP;
        private UserControl[] tabs;
        private Button[] navBtns;
        private int activeTab = 0;

        public MainForm()
        {
            InitUI();
        }

        private void InitUI()
        {
            Text = "Arena GameNet Pro";
            Size = new Size(1220, 740);
            MinimumSize = new Size(1220, 740);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.TextPri;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ── هدر ──
            pnlHeader = new Panel { Left=0, Top=0, Width=1220, Height=60, BackColor=Theme.Panel };
            // خط قرمز سمت چپ
            pnlHeader.Controls.Add(new Panel { Left=0, Top=0, Width=5, Height=60, BackColor=Theme.Accent });
            var lblTitle = new Label { Text="ARENA GAMENET PRO", Font=new Font("Segoe UI Black",17f,FontStyle.Bold), ForeColor=Theme.TextPri, Left=18, Top=13, AutoSize=true };
            var lblSub   = new Label { Text="arena gamenet pro", Font=new Font("Segoe UI",8.5f), ForeColor=Theme.TextDim, Left=21, Top=42, AutoSize=true };

            lblIP = new Label { Text="IP: " + Services.LanScanner.GetLocalIP(), Font=new Font("Consolas",9f), ForeColor=Theme.TextDim, Left=880, Top=42, AutoSize=true };
            lblStatus = new Label { Text="● Server: Stopped", Font=new Font("Segoe UI",9f,FontStyle.Bold), ForeColor=Theme.Accent, Left=880, Top=16, AutoSize=true };
            lblRcon   = new Label { Text="● RCON: —", Font=new Font("Segoe UI",9f), ForeColor=Theme.TextDim, Left=1030, Top=16, AutoSize=true };

            // دکمه NickName
            var btnNick = new FlatBtn("✏ Nickname", Color.FromArgb(60,40,0)) { Left=700, Top=14, Width=120, Height=32, Font=Theme.Small };
            btnNick.Click += (s,e) => ShowNickDialog();

            pnlHeader.Controls.AddRange(new Control[]{ lblTitle, lblSub, lblIP, lblStatus, lblRcon, btnNick });

            // ── ناوبری تب‌ها ──
            pnlNav = new Panel { Left=0, Top=60, Width=160, Height=680, BackColor=Color.FromArgb(14,14,22) };

            string[] tabNames = { "🏠  Dashboard", "🌐  Server Browser", "🖥  Create Server", "⌨  RCON Console", "⚡  Quick Actions", "⚙  Match Settings", "👥  Players & Bots", "🔧  Settings" };
            tabs = new UserControl[]
            {
                new DashboardTab(),
                new ServerBrowserTab(),
                new CreateServerTab(),
                new RconTab(),
                new QuickActionsTab(),
                new MatchSettingsTab(),
                new PlayersTab(),
                new SettingsTab()
            };
            navBtns = new Button[tabNames.Length];

            for (int i = 0; i < tabNames.Length; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text = tabNames[i], Left=0, Top=10 + i*72, Width=160, Height=64,
                    FlatStyle=FlatStyle.Flat, BackColor=Color.Transparent,
                    ForeColor=Theme.TextSec, Font=new Font("Segoe UI",9.5f),
                    TextAlign=ContentAlignment.MiddleLeft, Padding=new Padding(14,0,0,0),
                    Cursor=Cursors.Hand
                };
                btn.FlatAppearance.BorderSize=0;
                btn.FlatAppearance.MouseOverBackColor=Color.FromArgb(25,25,40);
                btn.Click += (s,e) => SwitchTab(idx);
                navBtns[i] = btn;
                pnlNav.Controls.Add(btn);
            }

            // ── محتوا ──
            pnlContent = new Panel { Left=160, Top=60, Width=1060, Height=680, BackColor=Theme.Bg };
            pnlContent.AutoScroll = true;
            foreach (var tab in tabs) { tab.Left=0; tab.Top=0; tab.Width=1040; tab.Height=680; tab.Visible=false; pnlContent.Controls.Add(tab); }

            // فوتر
            var footer = new Panel { Left=160, Top=710, Width=1060, Height=30, BackColor=Theme.Panel };
            footer.Controls.Add(new Label { Text="Arena GameNet Pro  v2.0  |  No Steam Required  |  LAN Only", ForeColor=Theme.TextDim, Font=Theme.Small, Left=10, Top=7, AutoSize=true });
            var btnExit = new FlatBtn("Exit", Theme.Accent) { Left=960, Top=2, Width=80, Height=26, Font=Theme.Small };
            btnExit.Click += (s,e) => { AppState.Server.Stop(); Application.Exit(); };
            footer.Controls.Add(btnExit);

            Controls.AddRange(new Control[]{ pnlHeader, pnlNav, pnlContent, footer });

            // اتصال event‌ها
            AppState.OnServerStateChanged += () =>
            {
                if (lblStatus.InvokeRequired) lblStatus.Invoke(new Action(() => UpdateHeaderStatus()));
                else UpdateHeaderStatus();
            };
            AppState.OnRconStateChanged += () =>
            {
                if (lblRcon.InvokeRequired) lblRcon.Invoke(new Action(() => UpdateHeaderStatus()));
                else UpdateHeaderStatus();
            };

            SwitchTab(0);
        }

        private void SwitchTab(int idx)
        {
            activeTab = idx;
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].Visible = (i == idx);
                navBtns[i].BackColor = (i == idx) ? Color.FromArgb(22,22,36) : Color.Transparent;
                navBtns[i].ForeColor = (i == idx) ? Theme.TextPri : Theme.TextSec;
                if (i == idx)
                {
                    // خط قرمز کنار آیتم فعال
                    navBtns[i].FlatAppearance.BorderColor = Theme.Accent;
                    navBtns[i].Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else
                {
                    navBtns[i].FlatAppearance.BorderColor = Color.Transparent;
                    navBtns[i].Font = new Font("Segoe UI", 9.5f);
                }
            }
        }

        private void UpdateHeaderStatus()
        {
            bool srv = AppState.ServerRunning;
            lblStatus.Text = srv ? "● Server: Running" : "● Server: Stopped";
            lblStatus.ForeColor = srv ? Theme.Green : Theme.Accent;
            bool rc = AppState.Rcon.IsConnected;
            lblRcon.Text = rc ? "● RCON: " + AppState.Rcon.CurrentIP : "● RCON: —";
            lblRcon.ForeColor = rc ? Theme.Green : Theme.TextDim;
        }

        private void ShowNickDialog()
        {
            var frm = new Form { Text="تغییر نام بازیکن", Size=new Size(420,190), StartPosition=FormStartPosition.CenterScreen, BackColor=Theme.Bg, FormBorderStyle=FormBorderStyle.FixedDialog, MaximizeBox=false };
            frm.Controls.Add(new Label { Text="نام دلخواه در CS2:", ForeColor=Theme.TextPri, Font=Theme.Body, Left=20, Top=20, AutoSize=true });
            var txt = new DarkInput { Left=20, Top=44, Width=360, Height=34, Text=AppState.Settings.PlayerName };
            var btnOk = new FlatBtn("ثبت", Theme.Accent) { Left=270, Top=100, Width=110, Height=36 };
            btnOk.Click += (s2,e2) =>
            {
                AppState.Settings.PlayerName = txt.Text;
                AppState.Settings.Save();
                AppState.Log($"✓ نام بازیکن تغییر کرد: {txt.Text}");
                frm.Close();
            };
            var btnCancel = new FlatBtn("انصراف", Color.FromArgb(40,40,60)) { Left=155, Top=100, Width=110, Height=36 };
            btnCancel.Click += (s2,e2) => frm.Close();
            frm.Controls.AddRange(new Control[]{ txt, btnOk, btnCancel });
            frm.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            AppState.Server.Stop();
            AppState.Rcon.Disconnect();
            base.OnFormClosing(e);
        }
    }
}
