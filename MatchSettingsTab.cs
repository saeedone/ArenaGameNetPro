using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class MatchSettingsTab : UserControl
    {
        public MatchSettingsTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Match Settings",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var lblFreeze = new Label { Text = "Freeze Time", Left = 30, Top = 65, AutoSize = true, ForeColor = Theme.TextSec };
            var txtFreeze = new DarkTextBox { Left = 30, Top = 90, Width = 120, Height = 34, Text = "5" };
            var btnFreeze = new FlatButton("Apply", Color.FromArgb(60, 60, 90)) { Left = 165, Top = 90, Width = 75, Height = 34 };

            var lblRounds = new Label { Text = "Max Rounds", Left = 280, Top = 65, AutoSize = true, ForeColor = Theme.TextSec };
            var txtRounds = new DarkTextBox { Left = 280, Top = 90, Width = 120, Height = 34, Text = "30" };
            var btnRounds = new FlatButton("Apply", Color.FromArgb(60, 60, 90)) { Left = 415, Top = 90, Width = 75, Height = 34 };

            var lblWarmup = new Label { Text = "Warmup Time", Left = 530, Top = 65, AutoSize = true, ForeColor = Theme.TextSec };
            var txtWarmup = new DarkTextBox { Left = 530, Top = 90, Width = 120, Height = 34, Text = "60" };
            var btnWarmup = new FlatButton("Apply", Color.FromArgb(60, 60, 90)) { Left = 665, Top = 90, Width = 75, Height = 34 };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFreeze); this.Controls.Add(txtFreeze); this.Controls.Add(btnFreeze);
            this.Controls.Add(lblRounds); this.Controls.Add(txtRounds); this.Controls.Add(btnRounds);
            this.Controls.Add(lblWarmup); this.Controls.Add(txtWarmup); this.Controls.Add(btnWarmup);
        }
    }
}