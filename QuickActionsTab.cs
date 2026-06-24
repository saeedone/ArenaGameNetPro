using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class QuickActionsTab : UserControl
    {
        public QuickActionsTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var btnStartMatch = new FlatButton("Start Match", Theme.Green) { Left = 30, Top = 75, Width = 200, Height = 48 };
            var btnRestart = new FlatButton("Restart Round", Color.FromArgb(200, 120, 50)) { Left = 250, Top = 75, Width = 200, Height = 48 };
            var btnSwapTeams = new FlatButton("Swap Teams", Color.FromArgb(80, 80, 150)) { Left = 470, Top = 75, Width = 200, Height = 48 };
            var btnKickBots = new FlatButton("Kick All Bots", Theme.Accent) { Left = 690, Top = 75, Width = 200, Height = 48 };

            var btnPause = new FlatButton("Pause Match", Color.FromArgb(180, 100, 30)) { Left = 30, Top = 140, Width = 200, Height = 48 };
            var btnUnpause = new FlatButton("Unpause Match", Color.FromArgb(50, 150, 80)) { Left = 250, Top = 140, Width = 200, Height = 48 };
            var btnScramble = new FlatButton("Scramble Teams", Color.FromArgb(120, 60, 120)) { Left = 470, Top = 140, Width = 200, Height = 48 };
            var btnWarmup = new FlatButton("Start Warmup", Theme.Blue) { Left = 690, Top = 140, Width = 200, Height = 48 };

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnStartMatch);
            this.Controls.Add(btnRestart);
            this.Controls.Add(btnSwapTeams);
            this.Controls.Add(btnKickBots);
            this.Controls.Add(btnPause);
            this.Controls.Add(btnUnpause);
            this.Controls.Add(btnScramble);
            this.Controls.Add(btnWarmup);
        }
    }
}