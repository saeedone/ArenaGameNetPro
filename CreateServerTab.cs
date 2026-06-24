using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class CreateServerTab : UserControl
    {
        private Settings settings;
        private GameLauncher launcher;

        public CreateServerTab(Settings s, GameLauncher l)
        {
            settings = s;
            launcher = l;
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Create Server",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var lblName = new Label { Text = "نام سرور", Left = 30, Top = 65, AutoSize = true, ForeColor = Theme.TextSec };
            var txtName = new DarkTextBox { Left = 30, Top = 90, Width = 400, Height = 36, Text = "Arena LAN Server" };

            var lblMap = new Label { Text = "مپ", Left = 460, Top = 65, AutoSize = true, ForeColor = Theme.TextSec };
            var cboMap = new DarkComboBox { Left = 460, Top = 90, Width = 400, Height = 36 };
            cboMap.Items.AddRange(new[] { "de_dust2", "de_mirage", "de_inferno", "de_nuke", "de_overpass", "de_ancient" });
            cboMap.SelectedIndex = 0;

            var lblPlayers = new Label { Text = "تعداد بازیکن", Left = 30, Top = 150, AutoSize = true, ForeColor = Theme.TextSec };
            var nudPlayers = new NumericUpDown { Left = 30, Top = 175, Width = 140, Height = 36, Minimum = 2, Maximum = 64, Value = 10 };

            var lblBots = new Label { Text = "تعداد بات", Left = 190, Top = 150, AutoSize = true, ForeColor = Theme.TextSec };
            var nudBots = new NumericUpDown { Left = 190, Top = 175, Width = 140, Height = 36, Minimum = 0, Maximum = 30, Value = 6 };

            var lblBotDiff = new Label { Text = "سختی بات", Left = 350, Top = 150, AutoSize = true, ForeColor = Theme.TextSec };
            var cboBotDiff = new DarkComboBox { Left = 350, Top = 175, Width = 170, Height = 36 };
            cboBotDiff.Items.AddRange(new[] { "Easy", "Normal", "Hard", "Expert" });
            cboBotDiff.SelectedIndex = 3;

            var lblSkin = new Label { Text = "اسکین‌ها", Left = 30, Top = 235, AutoSize = true, ForeColor = Theme.TextSec, Font = new Font("Segoe UI", 13f, FontStyle.Bold) };

            var chkGun = new CheckBox { Text = "گان", Left = 30, Top = 270, AutoSize = true, ForeColor = Theme.TextPri };
            var txtGun = new DarkTextBox { Left = 100, Top = 268, Width = 220, Height = 34, PlaceholderText = "Gun Collection ID" };

            var chkKnife = new CheckBox { Text = "نایف", Left = 340, Top = 270, AutoSize = true, ForeColor = Theme.TextPri };
            var txtKnife = new DarkTextBox { Left = 410, Top = 268, Width = 220, Height = 34, PlaceholderText = "Knife Collection ID" };

            var chkGlove = new CheckBox { Text = "دستکش", Left = 610, Top = 270, AutoSize = true, ForeColor = Theme.TextPri };
            var txtGlove = new DarkTextBox { Left = 700, Top = 268, Width = 220, Height = 34, PlaceholderText = "Glove Collection ID" };

            var chkAutoDownload = new CheckBox { Text = "دانلود خودکار اسکین", Left = 30, Top = 320, AutoSize = true, ForeColor = Theme.TextPri, Checked = true };

            var btnStart = new FlatButton("▶ شروع سرور", Theme.Green) { Left = 30, Top = 370, Width = 260, Height = 55, Font = new Font("Segoe UI", 15f, FontStyle.Bold) };
            btnStart.Click += (sender, e) => MessageBox.Show("در حال راه‌اندازی سرور...");

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblName); this.Controls.Add(txtName);
            this.Controls.Add(lblMap); this.Controls.Add(cboMap);
            this.Controls.Add(lblPlayers); this.Controls.Add(nudPlayers);
            this.Controls.Add(lblBots); this.Controls.Add(nudBots);
            this.Controls.Add(lblBotDiff); this.Controls.Add(cboBotDiff);
            this.Controls.Add(lblSkin);
            this.Controls.Add(chkGun); this.Controls.Add(txtGun);
            this.Controls.Add(chkKnife); this.Controls.Add(txtKnife);
            this.Controls.Add(chkGlove); this.Controls.Add(txtGlove);
            this.Controls.Add(chkAutoDownload);
            this.Controls.Add(btnStart);
        }
    }
}