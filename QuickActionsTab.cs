using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class QuickActionsTab : UserControl
    {
        private RichTextBox rtbLog;

        public QuickActionsTab()
        {
            BackColor = Theme.Bg;
            InitUI();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Quick Actions", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            // وضعیت RCON
            var pnlRcon = new CardBox(Theme.Blue) { Left=30, Top=72, Width=600, Height=44 };
            pnlRcon.Controls.Add(new Label { Text="RCON:", ForeColor=Theme.TextSec, Font=Theme.Label, Left=12, Top=14, AutoSize=true });
            var lblRcon = new Label { Text="—", ForeColor=Theme.TextSec, Font=Theme.Body, Left=60, Top=12, AutoSize=true };
            pnlRcon.Controls.Add(lblRcon);
            AppState.OnRconStateChanged += () =>
            {
                bool c = AppState.Rcon.IsConnected;
                if (lblRcon.InvokeRequired) lblRcon.Invoke(new Action(() => { lblRcon.Text = c ? "✓ " + AppState.Rcon.CurrentIP : "✗ Not connected"; lblRcon.ForeColor = c ? Theme.Green : Theme.Accent; }));
                else { lblRcon.Text = AppState.Rcon.IsConnected ? "✓ " + AppState.Rcon.CurrentIP : "✗ Not connected"; lblRcon.ForeColor = AppState.Rcon.IsConnected ? Theme.Green : Theme.Accent; }
            };
            Controls.Add(pnlRcon);

            int y = 130;
            Sec("MATCH CONTROLS", 30, y); y+=26;
            RBtn("Start Match",      "mp_warmup_end; mp_restartgame 1", Theme.Green,   30,  y, 155);
            RBtn("Restart Round",    "mp_restartgame 1",                  Color.FromArgb(120,60,20), 198, y, 140);
            RBtn("Swap Teams",       "mp_swapteams",                      Color.FromArgb(50,60,130), 350, y, 130);
            RBtn("Pause Match",      "mp_pause_match",                    Color.FromArgb(80,75,20),  492, y, 130);
            RBtn("Unpause Match",    "mp_unpause_match",                  Color.FromArgb(25,75,25),  634, y, 140);
            RBtn("Scramble Players", "mp_scrambleteams",                  Color.FromArgb(80,25,80),  786, y, 155);

            y+=55; Sec("WARMUP", 30, y); y+=26;
            RBtn("Start Warmup",  "mp_warmup_start",   Theme.Blue,                    30,  y, 150);
            RBtn("Stop Warmup",   "mp_warmup_end",     Theme.Accent,                  192, y, 140);
            RBtn("Pause Warmup",  "mp_warmuptime 9999",Color.FromArgb(35,75,115),     344, y, 140);

            y+=55; Sec("BOT CONTROLLERS", 30, y); y+=26;
            RBtn("Kick All Bots", "bot_kick",    Theme.Accent,                  30,  y, 145);
            RBtn("Add Bot CT",    "bot_add ct",  Theme.Blue,                    187, y, 135);
            RBtn("Add Bot T",     "bot_add t",   Theme.Gold,                    334, y, 125);
            RBtn("Fill With Bots","bot_quota 10; bot_add", Color.FromArgb(65,55,15), 471, y, 155);

            y+=55; Sec("ROUND BACKUP", 30, y); y+=26;
            RBtn("List Backups",    "listsaves",          Color.FromArgb(45,45,80),  30,  y, 140);
            RBtn("Start Recording", "tv_record match",    Theme.Blue,                182, y, 155);
            RBtn("Stop Recording",  "tv_stoprecord",      Theme.Accent,              349, y, 155);

            y+=55; Sec("KNIFE ROUND", 30, y); y+=26;
            RBtn("Start Knife Round","mp_warmup_end; mp_give_player_c4 0; mp_ct_default_melee weapon_knife; mp_t_default_melee weapon_knife; mp_restartgame 1",
                Color.FromArgb(110,50,0), 30, y, 190);

            y+=55;
            Controls.Add(new Label { Text="RCON HISTORY:", ForeColor=Theme.TextDim, Font=Theme.Label, Left=30, Top=y, AutoSize=true }); y+=18;
            rtbLog = new RichTextBox { Left=30, Top=y, Width=1100, Height=160, BackColor=Theme.LogBg, ForeColor=Theme.Green, Font=Theme.Mono, ReadOnly=true, BorderStyle=BorderStyle.FixedSingle };
            Controls.Add(rtbLog);
        }

        private void Sec(string t, int x, int y) =>
            Controls.Add(new SectionLabel(t) { Left=x, Top=y });

        private void RBtn(string label, string cmd, Color color, int x, int y, int w=130)
        {
            var btn = new FlatBtn(label, color) { Left=x, Top=y, Width=w, Height=36, Font=Theme.Small };
            btn.Click += (s,e) =>
            {
                string resp = AppState.Exec(cmd);
                Log($"► {cmd}");
                if (!string.IsNullOrWhiteSpace(resp)) Log(resp.Trim());
            };
            Controls.Add(btn);
        }

        private void Log(string msg)
        {
            if (rtbLog == null) return;
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action<string>(Log), msg); return; }
            rtbLog.SelectionColor = msg.StartsWith("►") ? Theme.Gold : msg.Contains("✗") ? Theme.Accent : Theme.TextPri;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}]  {msg}\n");
            rtbLog.ScrollToCaret();
        }
    }
}
