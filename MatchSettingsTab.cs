using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class MatchSettingsTab : UserControl
    {
        private RichTextBox rtbLog;

        public MatchSettingsTab()
        {
            BackColor = Theme.Bg;
            InitUI();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Match Settings", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            int y = 75;
            Sec("MATCH SETTINGS", 30, y); y += 26;
            Field("Freeze Time (sec)",      "mp_freezetime",          "5",     30,  y);
            Field("Max Rounds",             "mp_maxrounds",           "30",    200, y);
            Field("Warmup Time (sec)",      "mp_warmuptime",          "60",    370, y);
            Field("Round Time Defuse (min)","mp_roundtime_defuse",    "1.92",  540, y);
            Field("Buy Time (sec)",         "mp_buytime",             "15",    710, y);
            Field("Start Money",            "mp_startmoney",          "800",   880, y);

            y += 85;
            Field("Gravity",               "sv_gravity",              "800",   30,  y);
            Field("Restart Delay (sec)",   "mp_round_restart_delay",  "5",     200, y);
            Field("Max Ping",              "sv_maxping",              "0",     370, y);
            Field("Time Limit (min)",      "mp_timelimit",            "0",     540, y);

            y += 85; Sec("SERVER PASSWORD", 30, y); y += 26;
            var txtPass = new DarkInput { Left=30, Top=y, Width=200, Height=30 };
            Controls.Add(txtPass);
            var btnSet = new FlatBtn("Set Password", Theme.Blue) { Left=242, Top=y-2, Width=150, Height=34, Font=Theme.Small };
            var btnClear = new FlatBtn("Clear Password", Color.FromArgb(40,40,60)) { Left=404, Top=y-2, Width=150, Height=34, Font=Theme.Small };
            btnSet.Click   += (s,e) => Exec("sv_password " + txtPass.Text);
            btnClear.Click += (s,e) => { txtPass.Clear(); Exec("sv_password \"\""); };
            Controls.Add(btnSet); Controls.Add(btnClear); Controls.Add(txtPass);

            y += 55; Sec("TEAM NAMES", 30, y); y += 26;
            var txtCT = new DarkInput { Left=30, Top=y, Width=200, Height=30, PlaceholderText="CT Team Name" };
            var txtT  = new DarkInput { Left=242, Top=y, Width=200, Height=30, PlaceholderText="T Team Name" };
            var btnTeam = new FlatBtn("Apply", Theme.Blue) { Left=454, Top=y-2, Width=100, Height=34, Font=Theme.Small };
            btnTeam.Click += (s,e) => { Exec("mp_teamname_1 " + txtCT.Text); Exec("mp_teamname_2 " + txtT.Text); };
            Controls.Add(txtCT); Controls.Add(txtT); Controls.Add(btnTeam);

            y += 55; Sec("OVERTIME SETTINGS", 30, y); y += 26;
            Field("Overtime Max Rounds",    "mp_overtime_maxrounds",   "6",     30,  y);
            Field("Overtime Start Money",   "mp_overtime_startmoney",  "16000", 200, y);
            Toggle("Enable Overtime",       "mp_overtime_enable",                370, y);

            y += 85; Sec("GAMEPLAY TOGGLES", 30, y); y += 26;
            Toggle("AllTalk",          "sv_full_alltalk",    30,  y);
            Toggle("Friendly Fire",    "mp_friendlyfire",    230, y);
            Toggle("SV Cheats",        "sv_cheats",          430, y);
            Toggle("Auto Team Balance","mp_autoteambalance", 630, y);
            Toggle("Buy Anywhere",     "mp_buy_anywhere",    830, y);

            y += 80;
            Controls.Add(new Label { Text="RCON RESPONSE:", ForeColor=Theme.TextDim, Font=Theme.Label, Left=30, Top=y, AutoSize=true }); y+=18;
            rtbLog = new RichTextBox { Left=30, Top=y, Width=1100, Height=130, BackColor=Theme.LogBg, ForeColor=Theme.Green, Font=Theme.Mono, ReadOnly=true, BorderStyle=BorderStyle.FixedSingle };
            Controls.Add(rtbLog);
        }

        private void Field(string label, string cvar, string def, int x, int y)
        {
            Controls.Add(new FieldLabel(label) { Left=x, Top=y });
            var txt = new DarkInput { Left=x, Top=y+18, Width=120, Height=28, Text=def };
            var btn = new FlatBtn("Apply", Color.FromArgb(40,40,65)) { Left=x+124, Top=y+17, Width=56, Height=30, Font=Theme.Small };
            btn.Click += (s,e) => Exec($"{cvar} {txt.Text}");
            Controls.Add(txt); Controls.Add(btn);
        }

        private void Toggle(string label, string cvar, int x, int y)
        {
            Controls.Add(new FieldLabel(label) { Left=x, Top=y });
            var on  = new FlatBtn("On",  Theme.Green)  { Left=x,    Top=y+18, Width=70, Height=30, Font=Theme.Small };
            var off = new FlatBtn("Off", Theme.Accent) { Left=x+74, Top=y+18, Width=70, Height=30, Font=Theme.Small };
            on.Click  += (s,e) => Exec($"{cvar} 1");
            off.Click += (s,e) => Exec($"{cvar} 0");
            Controls.Add(on); Controls.Add(off);
        }

        private void Sec(string t, int x, int y) =>
            Controls.Add(new SectionLabel(t) { Left=x, Top=y });

        private void Exec(string cmd)
        {
            string resp = AppState.Exec(cmd);
            if (rtbLog == null) return;
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action<string>(Exec), cmd); return; }
            rtbLog.SelectionColor = Theme.Gold;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}]  ► {cmd}\n");
            if (!string.IsNullOrWhiteSpace(resp)) { rtbLog.SelectionColor = Theme.TextPri; rtbLog.AppendText(resp.Trim() + "\n"); }
            rtbLog.ScrollToCaret();
        }
    }
}
