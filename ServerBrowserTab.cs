using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class ServerBrowserTab : UserControl
    {
        public ServerBrowserTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "Server Browser",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var btnRefresh = new FlatButton("Refresh", Theme.Accent) { Left = 30, Top = 65, Width = 140, Height = 38 };
            var txtSearch = new DarkTextBox { Left = 190, Top = 70, Width = 500, Height = 34, PlaceholderText = "Search..." };

            var listServers = new ListBox
            {
                Left = 30,
                Top = 120,
                Width = 1050,
                Height = 460,
                BackColor = Theme.Card,
                ForeColor = Theme.TextPri,
                Font = new Font("Segoe UI", 11.5f)
            };

            listServers.Items.Add("192.168.1.105   |   de_dust2     |   8/10   |   LAN");
            listServers.Items.Add("192.168.1.112   |   de_mirage    |   5/10   |   LAN");
            listServers.Items.Add("192.168.1.98    |   de_inferno   |   12/12  |   LAN");

            this.Controls.Add(lblTitle);
            this.Controls.Add(btnRefresh);
            this.Controls.Add(txtSearch);
            this.Controls.Add(listServers);
        }
    }
}