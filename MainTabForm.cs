using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class MainTabForm : Form
    {
        private TabControl mainTabControl;
        private Settings settings;
        private GameLauncher launcher;

        public MainTabForm()
        {
            settings = Settings.Load();
            launcher = new GameLauncher();
            InitUI();
        }

        private void InitUI()
        {
            this.Text = "Arena GameNet Pro";
            this.Size = new Size(1280, 780);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Theme.Bg;
            this.ForeColor = Theme.TextPri;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = true;

            // هدر بالا (لوکس و حرفه‌ای)
            var header = new Panel
            {
                Height = 62,
                Dock = DockStyle.Top,
                BackColor = Theme.Panel
            };

            var lblLogo = new Label
            {
                Text = "ARENA GAMENET PRO",
                Font = new Font("Segoe UI Black", 19f, FontStyle.Bold),
                ForeColor = Theme.Accent,
                Left = 25,
                Top = 17,
                AutoSize = true
            };

            var lblVersion = new Label
            {
                Text = "v2.0  •  Professional LAN Launcher",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Theme.TextDim,
                Left = 295,
                Top = 23,
                AutoSize = true
            };

            header.Controls.Add(lblLogo);
            header.Controls.Add(lblVersion);

            // TabControl اصلی
            mainTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.Normal,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ItemSize = new Size(155, 42),
                BackColor = Theme.Bg,
                ForeColor = Theme.TextPri
            };

            // اضافه کردن ۸ تب حرفه‌ای
            mainTabControl.TabPages.Add(CreateTab("Dashboard", new DashboardTab()));
            mainTabControl.TabPages.Add(CreateTab("Server Browser", new ServerBrowserTab()));
            mainTabControl.TabPages.Add(CreateTab("Create Server", new CreateServerTab(settings, launcher)));
            mainTabControl.TabPages.Add(CreateTab("RCON Console", new RconConsoleTab()));
            mainTabControl.TabPages.Add(CreateTab("Quick Actions", new QuickActionsTab()));
            mainTabControl.TabPages.Add(CreateTab("Match Settings", new MatchSettingsTab()));
            mainTabControl.TabPages.Add(CreateTab("Players & Bots", new PlayersBotsTab()));
            mainTabControl.TabPages.Add(CreateTab("Settings", new SettingsTab(settings)));

            this.Controls.Add(header);
            this.Controls.Add(mainTabControl);
        }

        private TabPage CreateTab(string title, UserControl control)
        {
            TabPage tab = new TabPage(title)
            {
                BackColor = Theme.Bg,
                ForeColor = Theme.TextPri
            };
            control.Dock = DockStyle.Fill;
            tab.Controls.Add(control);
            return tab;
        }
    }
}