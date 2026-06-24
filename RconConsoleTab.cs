using System.Drawing;
using System.Windows.Forms;

namespace ArenaGameNet
{
    public class RconConsoleTab : UserControl
    {
        public RconConsoleTab()
        {
            this.BackColor = Theme.Bg;

            var lblTitle = new Label
            {
                Text = "RCON Console",
                Font = new Font("Segoe UI", 23f, FontStyle.Bold),
                ForeColor = Theme.TextPri,
                Left = 30,
                Top = 18,
                AutoSize = true
            };

            var rtbConsole = new RichTextBox
            {
                Left = 30,
                Top = 65,
                Width = 1000,
                Height = 440,
                BackColor = Theme.LogBg,
                ForeColor = Theme.Green,
                Font = Theme.FontMono,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            var txtCommand = new DarkTextBox 
            { 
                Left = 30, 
                Top = 525, 
                Width = 780, 
                Height = 38,
                PlaceholderText = "Type RCON command..."
            };

            var btnSend = new FlatButton("Send", Theme.Blue) { Left = 830, Top = 525, Width = 110, Height = 38 };
            var btnClear = new FlatButton("Clear", Theme.Accent) { Left = 950, Top = 525, Width = 90, Height = 38 };

            btnSend.Click += (s, e) =>
            {
                if (!string.IsNullOrWhiteSpace(txtCommand.Text))
                {
                    rtbConsole.AppendText($"> {txtCommand.Text}\n");
                    txtCommand.Clear();
                }
            };

            btnClear.Click += (s, e) => rtbConsole.Clear();

            this.Controls.Add(lblTitle);
            this.Controls.Add(rtbConsole);
            this.Controls.Add(txtCommand);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnClear);
        }
    }
}