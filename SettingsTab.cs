using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class SettingsTab : UserControl
    {
        private Settings settings;

        public SettingsTab(Settings s)
        {
            settings = s;
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var lblPath = new Label { Text = "مسیر نصب بازی (CS2)", Left = 30, Top = 70, AutoSize = true, ForeColor = Theme.TextSec };
            var txtPath = new DarkTextBox { Left = 30, Top = 100, Width = 600, Height = 36, Text = settings.GamePath };

            var btnBrowse = new FlatButton("Browse", Color.FromArgb(60, 60, 90)) { Left = 650, Top = 100, Width = 120, Height = 36 };
            var btnSave = new FlatButton("ذخیره تنظیمات", Theme.Green) { Left = 30, Top = 165, Width = 220, Height = 48 };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblPath);
            this.Controls.Add(txtPath);
            this.Controls.Add(btnBrowse);
            this.Controls.Add(btnSave);
        }
    }
}