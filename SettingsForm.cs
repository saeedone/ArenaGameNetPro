using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class SettingsForm : Form
    {
        private Settings settings;
        private TextBox txtGamePath, txtPlayerName;
        private CheckBox chkNoSteam;

        public SettingsForm(Settings s)
        {
            settings = s;
            InitUI();
        }

        private void InitUI()
        {
            Text = "تنظیمات";
            Size = new Size(540, 320);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Bg;
            ForeColor = Theme.TextPri;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            var pnlHeader = new Panel { Left = 0, Top = 0, Width = 540, Height = 50, BackColor = Theme.Panel };
            pnlHeader.Controls.Add(new Label { Text = "Launcher Settings", Font = new Font("Segoe UI", 12f, FontStyle.Bold), ForeColor = Theme.TextPri, Left = 15, Top = 12, AutoSize = true, RightToLeft = RightToLeft.No });

            var pnl = new CardPanel { Left = 10, Top = 60, Width = 510, Height = 190 };

            pnl.Controls.Add(new LabelSmall("مسیر بازی (CS2 Installation Folder)") { Left = 10, Top = 10 });
            txtGamePath = new DarkTextBox { Left = 10, Top = 28, Width = 390, Height = 26, Text = settings.GamePath, RightToLeft = RightToLeft.No };

            var btnBrowse = new FlatButton("Browse", Color.FromArgb(40, 40, 65)) { Left = 408, Top = 28, Width = 80, Height = 26, Font = Theme.FontSmall };
            btnBrowse.Click += (s, e) =>
            {
                using var d = new FolderBrowserDialog { Description = "پوشه Counter-Strike Global Offensive رو انتخاب کن" };
                if (d.ShowDialog() == DialogResult.OK) txtGamePath.Text = d.SelectedPath;
            };

            pnl.Controls.Add(new LabelSmall("نام بازیکن در CS2") { Left = 10, Top = 65 });
            txtPlayerName = new DarkTextBox { Left = 10, Top = 83, Width = 250, Height = 26, Text = settings.PlayerName, RightToLeft = RightToLeft.No };

            chkNoSteam = new CheckBox { Text = "NoSteam Mode (بدون Steam اجرا شود)", Left = 10, Top = 125, AutoSize = true, ForeColor = Theme.TextPri, Font = Theme.FontSub, Checked = settings.NoSteamMode };

            pnl.Controls.AddRange(new Control[] { txtGamePath, btnBrowse, txtPlayerName, chkNoSteam });

            var btnSave = new FlatButton("💾 ذخیره", Theme.Green) { Left = 10, Top = 260, Width = 150, Height = 36 };
            btnSave.ForeColor = Color.Black;
            btnSave.Click += (s, e) =>
            {
                settings.GamePath = txtGamePath.Text;
                settings.PlayerName = txtPlayerName.Text;
                settings.NoSteamMode = chkNoSteam.Checked;
                settings.Save();
                MessageBox.Show("تنظیمات ذخیره شد ✓", "Arena GameNet Pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            };

            var btnClose = new FlatButton("بستن", Theme.Accent) { Left = 175, Top = 260, Width = 100, Height = 36 };
            btnClose.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { pnlHeader, pnl, btnSave, btnClose });
        }
    }
}