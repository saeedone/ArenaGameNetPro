using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AGP.Services;

namespace AGP.Tabs
{
    public class ServerBrowserTab : UserControl
    {
        private FlowLayoutPanel flowList;
        private Label lblCount, lblScanning;
        private TextBox txtSearch;
        private List<ServerEntry> _entries = new List<ServerEntry>();
        private ServerEntry _selected;

        public ServerBrowserTab()
        {
            BackColor = Theme.Bg;
            InitUI();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Server Browser", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            // نوار ابزار
            var toolbar = new Panel { Left = 30, Top = 72, Width = 1100, Height = 46, BackColor = Theme.Panel };
            var btnRefresh = new FlatBtn("🔄  Refresh", Theme.Accent) { Left = 8, Top = 6, Width = 120, Height = 34 };
            btnRefresh.Click += (s,e) => DoScan();
            txtSearch = new DarkInput { Left = 140, Top = 9, Width = 400, Height = 28 };
            txtSearch.PlaceholderText = "Search by name, map, IP...";
            txtSearch.TextChanged += (s,e) => FilterList();
            lblCount = new Label { Text = "0 servers", ForeColor = Theme.TextSec, Font = Theme.Small, Left = 560, Top = 14, AutoSize = true };
            lblScanning = new Label { Text = "", ForeColor = Theme.Gold, Font = Theme.Small, Left = 680, Top = 14, AutoSize = true };
            toolbar.Controls.AddRange(new Control[] { btnRefresh, txtSearch, lblCount, lblScanning });

            // هدر لیست
            var hdr = new Panel { Left = 30, Top = 122, Width = 1100, Height = 30, BackColor = Color.FromArgb(18,18,28) };
            void HdrLbl(string t, int x, int w) => hdr.Controls.Add(new Label { Text = t, ForeColor = Theme.TextDim, Font = Theme.Label, Left = x, Top = 8, Width = w, AutoSize = false });
            HdrLbl("SERVER NAME", 10, 300); HdrLbl("MAP", 320, 160); HdrLbl("PLAYERS", 490, 100); HdrLbl("IP", 600, 200); HdrLbl("STATUS", 810, 100);

            flowList = new FlowLayoutPanel
            {
                Left = 30, Top = 154, Width = 1100, Height = 430,
                BackColor = Theme.Bg, FlowDirection = FlowDirection.TopDown,
                WrapContents = false, AutoScroll = true
            };

            // پنل کانکت
            var pnlJoin = new CardBox(Theme.Blue) { Left = 30, Top = 594, Width = 1100, Height = 62 };
            pnlJoin.Controls.Add(new FieldLabel("IP میزبان (یا از لیست انتخاب کن)") { Left = 12, Top = 8 });
            var txtIP = new DarkInput { Left = 12, Top = 26, Width = 260, Height = 28 };
            txtIP.Text = AppState.Settings.LastIP;
            var btnJoin = new FlatBtn("🎮  Connect & Launch", Theme.Green) { Left = 282, Top = 22, Width = 200, Height = 34 };
            btnJoin.ForeColor = Color.Black;
            btnJoin.Click += (s,e) =>
            {
                string ip = _selected?.IP ?? txtIP.Text.Trim();
                if (string.IsNullOrEmpty(ip)) { AppState.Log("✗ IP وارد کن یا سرور انتخاب کن"); return; }
                if (string.IsNullOrEmpty(AppState.Settings.GamePath)) { AppState.Log("✗ مسیر بازی در Settings تنظیم نشده"); return; }
                AppState.Settings.LastIP = ip; AppState.Settings.Save();
                AppState.Server.LaunchClient(AppState.Settings.GamePath, ip);
            };
            var btnScan = new FlatBtn("🔍  Scan LAN", Color.FromArgb(100,70,0)) { Left = 494, Top = 22, Width = 130, Height = 34 };
            btnScan.Click += (s,e) => DoScan();
            pnlJoin.Controls.AddRange(new Control[] { txtIP, btnJoin, btnScan });

            Controls.AddRange(new Control[] { toolbar, hdr, flowList, pnlJoin });
            EmptyMessage("Refresh رو بزن تا سرورها پیدا بشن");
        }

        private void DoScan()
        {
            _entries.Clear();
            flowList.Controls.Clear();
            lblScanning.Text = "در حال اسکن...";
            lblCount.Text = "0 servers";

            new Thread(() =>
            {
                var found = LanScanner.ScanLAN(27015, msg => Invoke(new Action(() => lblScanning.Text = msg)));
                Invoke(new Action(() =>
                {
                    _entries = found;
                    lblScanning.Text = "";
                    lblCount.Text = found.Count + " server" + (found.Count != 1 ? "s" : "");
                    FilterList();
                    if (found.Count == 0) EmptyMessage("هیچ سروری پیدا نشد");
                }));
            }) { IsBackground = true }.Start();
        }

        private void FilterList()
        {
            string q = txtSearch.Text.Trim().ToLower();
            flowList.Controls.Clear();
            foreach (var e in _entries)
            {
                if (!string.IsNullOrEmpty(q) && !e.Name.ToLower().Contains(q) && !e.Map.ToLower().Contains(q) && !e.IP.Contains(q)) continue;
                flowList.Controls.Add(MakeCard(e));
            }
        }

        private Panel MakeCard(ServerEntry e)
        {
            var card = new Panel { Width = 1082, Height = 52, BackColor = Theme.Card, Margin = new Padding(0,0,0,3), Cursor = Cursors.Hand };
            card.Paint += (s,ev) => ev.Graphics.DrawRectangle(new Pen(Theme.Border), 0, 0, card.Width-1, card.Height-1);

            void Lbl(string t, int x, int w, Color c, Font f) =>
                card.Controls.Add(new Label { Text = t, Left = x, Top = 16, Width = w, ForeColor = c, Font = f, AutoSize = false });

            Lbl(e.Name, 10, 300, Theme.TextPri, new Font("Segoe UI", 10f, FontStyle.Bold));
            Lbl(e.Map,  320, 160, Theme.TextSec, Theme.Body);
            Lbl($"{e.Players}/{e.MaxPlayers}", 490, 100, Theme.Gold, Theme.Body);
            Lbl(e.IP + ":" + e.Port, 600, 200, Theme.Blue, Theme.Mono);

            var tagLAN = new Label { Text = " LAN ", Left = 810, Top = 14, AutoSize = true, ForeColor = Theme.Blue, BackColor = Color.FromArgb(20,50,90), Font = Theme.Label, Padding = new Padding(4,2,4,2) };
            card.Controls.Add(tagLAN);

            var btnSel = new FlatBtn(_selected == e ? "✓ Selected" : "Select", _selected == e ? Theme.Green : Theme.Accent) { Left = 980, Top = 10, Width = 90, Height = 30, Font = Theme.Small };
            btnSel.Click += (s,ev) => { _selected = e; FilterList(); AppState.Log("✓ انتخاب شد: " + e.IP); };
            card.Controls.Add(btnSel);

            return card;
        }

        private void EmptyMessage(string msg)
        {
            flowList.Controls.Clear();
            flowList.Controls.Add(new Label { Text = msg, ForeColor = Theme.TextDim, Font = Theme.Body, Width = 1082, Height = 50, TextAlign = ContentAlignment.MiddleCenter });
        }
    }
}
