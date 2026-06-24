using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class DashboardTab : UserControl
    {
        public DashboardTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 24f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 40,
                Top = 25,
                AutoSize = true
            };

            var pnlStatus = new Panel
            {
                Left = 40,
                Top = 85,
                Width = 420,
                Height = 120,
                BackColor = Theme.Card
            };
            pnlStatus.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Theme.Border), 0, 0, pnlStatus.Width - 1, pnlStatus.Height - 1);

            var lblStatus = new Label
            {
                Text = "● Server Status: Ready",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Theme.Green,
                Left = 25,
                Top = 22,
                AutoSize = true
            };

            var lblVersion = new Label
            {
                Text = "Arena GameNet Pro v2.0  •  LAN Mode",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Theme.TextSec,
                Left = 25,
                Top = 60,
                AutoSize = true
            };

            pnlStatus.Controls.Add(lblStatus);
            pnlStatus.Controls.Add(lblVersion);

            var pnlInfo = new Panel
            {
                Left = 490,
                Top = 85,
                Width = 420,
                Height = 120,
                BackColor = Theme.Card
            };
            pnlInfo.Paint += (s, e) => e.Graphics.DrawRectangle(new Pen(Theme.Border), 0, 0, pnlInfo.Width - 1, pnlInfo.Height - 1);

            var lblInfo = new Label
            {
                Text = "Ready to Host or Join Servers\nNetwork Discovery: Active",
                Font = new Font("Segoe UI", 13f),
                ForeColor = Theme.TextSec,
                Left = 25,
                Top = 35,
                AutoSize = true
            };

            pnlInfo.Controls.Add(lblInfo);

            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlStatus);
            this.Controls.Add(pnlInfo);
        }
    }
}