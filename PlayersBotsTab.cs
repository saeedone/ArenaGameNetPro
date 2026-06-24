using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class PlayersBotsTab : UserControl
    {
        public PlayersBotsTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Players & Bots",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var btnKickAll = new FlatButton("Kick All Bots", Theme.Accent) { Left = 30, Top = 70, Width = 200, Height = 44 };
            var btnAddBotCT = new FlatButton("Add Bot CT", Theme.Blue) { Left = 250, Top = 70, Width = 200, Height = 44 };
            var btnAddBotT = new FlatButton("Add Bot T", Color.FromArgb(200, 160, 50)) { Left = 470, Top = 70, Width = 200, Height = 44 };

            var listPlayers = new ListBox
            {
                Left = 30,
                Top = 130,
                Width = 850,
                Height = 380,
                BackColor = Theme.Card,
                ForeColor = Theme.TextPri,
                Font = new Font("Segoe UI", 11.5f)
            };

            listPlayers.Items.Add("shayan (CT) - 45ms - Active");
            listPlayers.Items.Add("Player2 (T) - 32ms - Active");

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnKickAll);
            this.Controls.Add(btnAddBotCT);
            this.Controls.Add(btnAddBotT);
            this.Controls.Add(listPlayers);
        }
    }
}