using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class PlayersTab : UserControl
    {
        private FlowLayoutPanel flowPlayers;
        private RichTextBox rtbLog;
        private System.Windows.Forms.Timer tmrRefresh;

        public PlayersTab()
        {
            BackColor = Theme.Bg;
            InitUI();
            tmrRefresh = new System.Windows.Forms.Timer { Interval = 5000 };
            tmrRefresh.Tick += (s,e) => RefreshPlayers();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Players & Bots", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            // نوار ابزار
            var toolbar = new Panel { Left=30, Top=72, Width=1100, Height=44, BackColor=Theme.Panel };
            var btnRefresh = new FlatBtn("🔄  Refresh Players", Theme.Blue) { Left=8, Top=6, Width=180, Height=32, Font=Theme.Small };
            btnRefresh.Click += (s,e) => RefreshPlayers();
            var chkAuto = new CheckBox { Text="Auto Refresh (5s)", Left=205, Top=10, AutoSize=true, ForeColor=Theme.TextSec, Font=Theme.Small, Checked=false };
            chkAuto.CheckedChanged += (s,e) => { if(chkAuto.Checked) tmrRefresh.Start(); else tmrRefresh.Stop(); };
            toolbar.Controls.AddRange(new Control[]{ btnRefresh, chkAuto });
            Controls.Add(toolbar);

            // لیست
            Controls.Add(new SectionLabel("CONNECTED PLAYERS") { Left=30, Top=126 });
            flowPlayers = new FlowLayoutPanel { Left=30, Top=144, Width=750, Height=390, BackColor=Theme.Bg, FlowDirection=FlowDirection.TopDown, WrapContents=false, AutoScroll=true };
            flowPlayers.Controls.Add(new Label { Text="Refresh رو بزن", ForeColor=Theme.TextDim, Font=Theme.Body, Width=740, Height=40, TextAlign=ContentAlignment.MiddleCenter });
            Controls.Add(flowPlayers);

            // پنل مدیریت
            var pnlAdmin = new CardBox { Left=800, Top=126, Width=330, Height=408 };
            pnlAdmin.Controls.Add(new SectionLabel("PLAYER ADMIN") { Left=12, Top=10 });

            pnlAdmin.Controls.Add(new FieldLabel("Kick by Name") { Left=12, Top=30 });
            var txtKick = new DarkInput { Left=12, Top=48, Width=250, Height=30 };
            var btnKick = new FlatBtn("Kick", Theme.Accent) { Left=12, Top=86, Width=120, Height=32, Font=Theme.Small };
            btnKick.Click += (s,e) => { if(!string.IsNullOrEmpty(txtKick.Text)) Exec("kickid " + txtKick.Text); };

            pnlAdmin.Controls.Add(new FieldLabel("Send Chat Message") { Left=12, Top=132 });
            var txtMsg = new DarkInput { Left=12, Top=150, Width=250, Height=30 };
            var btnMsg = new FlatBtn("Send", Theme.Blue) { Left=12, Top=188, Width=120, Height=32, Font=Theme.Small };
            btnMsg.Click += (s,e) => { if(!string.IsNullOrEmpty(txtMsg.Text)) { Exec("say " + txtMsg.Text); txtMsg.Clear(); } };

            pnlAdmin.Controls.Add(new SectionLabel("BOT CONTROLLERS") { Left=12, Top=238 });
            var btnKickBots = new FlatBtn("Kick All Bots", Theme.Accent) { Left=12, Top=258, Width=145, Height=32, Font=Theme.Small };
            var btnAddCT    = new FlatBtn("Add Bot CT",    Theme.Blue)   { Left=12, Top=298, Width=145, Height=32, Font=Theme.Small };
            var btnAddT     = new FlatBtn("Add Bot T",     Theme.Gold)   { Left=165, Top=258, Width=145, Height=32, Font=Theme.Small };
            var btnFill     = new FlatBtn("Fill Bots",     Color.FromArgb(60,50,10)) { Left=165, Top=298, Width=145, Height=32, Font=Theme.Small };
            btnKickBots.Click += (s,e) => Exec("bot_kick");
            btnAddCT.Click    += (s,e) => Exec("bot_add ct");
            btnAddT.Click     += (s,e) => Exec("bot_add t");
            btnFill.Click     += (s,e) => Exec("bot_quota 10");

            pnlAdmin.Controls.AddRange(new Control[]{ txtKick, btnKick, txtMsg, btnMsg, btnKickBots, btnAddCT, btnAddT, btnFill });
            Controls.Add(pnlAdmin);

            // لاگ
            Controls.Add(new SectionLabel("RCON LOG") { Left=30, Top=545 });
            rtbLog = new RichTextBox { Left=30, Top=562, Width=1100, Height=100, BackColor=Theme.LogBg, ForeColor=Theme.Green, Font=Theme.Mono, ReadOnly=true, BorderStyle=BorderStyle.FixedSingle };
            Controls.Add(rtbLog);
        }

        private void RefreshPlayers()
        {
            string resp = AppState.Rcon.Exec("status");
            if (InvokeRequired) { Invoke(new Action(RefreshPlayers)); return; }
            flowPlayers.Controls.Clear();
            if (string.IsNullOrWhiteSpace(resp)) { flowPlayers.Controls.Add(new Label { Text="RCON متصل نیست", ForeColor=Theme.Accent, Font=Theme.Body, Width=740, Height=40, TextAlign=ContentAlignment.MiddleCenter }); return; }

            int count = 0;
            foreach (var line in resp.Split('\n'))
            {
                string t = line.Trim();
                if (!t.StartsWith("#")) continue;
                var parts = t.Split(new[]{' ','\t'}, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5) continue;
                count++;
                string id = parts[0].TrimStart('#');
                string name = parts.Length > 2 ? parts[2].Trim('"') : "?";
                string ping = parts.Length > 4 ? parts[4] : "0";
                string state = parts.Length > 5 ? parts[5] : "active";
                bool isBot = t.Contains("BOT");

                var card = new Panel { Width=740, Height=50, BackColor=Theme.Card, Margin=new Padding(0,0,0,3), Cursor=Cursors.Default };
                card.Paint += (s,ev) => ev.Graphics.DrawRectangle(new Pen(Theme.Border), 0, 0, card.Width-1, card.Height-1);
                card.Controls.Add(new Label { Text=isBot?"🤖":"👤", Left=10, Top=14, AutoSize=true, Font=new Font("Segoe UI",13f) });
                card.Controls.Add(new Label { Text=name, Left=40, Top=14, Width=280, ForeColor=Theme.TextPri, Font=new Font("Segoe UI",10f,FontStyle.Bold), AutoSize=false });
                card.Controls.Add(new Label { Text="ping: "+ping, Left=330, Top=16, ForeColor=Theme.TextSec, Font=Theme.Small, AutoSize=true });
                card.Controls.Add(new Label { Text=state, Left=430, Top=16, ForeColor=state=="active"?Theme.Green:Theme.Gold, Font=Theme.Small, AutoSize=true });
                if (!isBot)
                {
                    var btnK = new FlatBtn("Kick", Theme.Accent) { Left=620, Top=10, Width=70, Height=28, Font=Theme.Small };
                    string capId = id;
                    btnK.Click += (s,ev) => Exec("kick #" + capId);
                    card.Controls.Add(btnK);
                }
                flowPlayers.Controls.Add(card);
            }
            if (count == 0) flowPlayers.Controls.Add(new Label { Text="هیچ بازیکنی متصل نیست", ForeColor=Theme.TextDim, Font=Theme.Body, Width=740, Height=40, TextAlign=ContentAlignment.MiddleCenter });
            Log($"✓ {count} بازیکن پیدا شد");
        }

        private void Exec(string cmd) { string r = AppState.Exec(cmd); Log($"► {cmd}"); if (!string.IsNullOrWhiteSpace(r)) Log(r.Trim()); }

        private void Log(string msg)
        {
            if (rtbLog.InvokeRequired) { rtbLog.Invoke(new Action<string>(Log), msg); return; }
            rtbLog.SelectionColor = msg.StartsWith("►") ? Theme.Gold : msg.Contains("✗") ? Theme.Accent : Theme.Green;
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}]  {msg}\n");
            rtbLog.ScrollToCaret();
        }

        protected override void OnHandleDestroyed(EventArgs e) { tmrRefresh?.Stop(); base.OnHandleDestroyed(e); }
    }
}
