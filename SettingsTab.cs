using System;
using System.Drawing;
using System.Windows.Forms;

namespace AGP.Tabs
{
    public class SettingsTab : UserControl
    {
        private DarkInput txtGamePath, txtPlayerName;
        private CheckBox chkNoSteam;
        private Label lblSaved;

        public SettingsTab()
        {
            BackColor = Theme.Bg;
            InitUI();
        }

        private void InitUI()
        {
            Controls.Add(new Label { Text = "Settings", Font = Theme.H1, ForeColor = Theme.TextPri, Left = 30, Top = 20, AutoSize = true });

            // مسیر بازی
            var cPath = new CardBox(Theme.Accent) { Left=30, Top=75, Width=950, Height=90 };
            cPath.Controls.Add(new SectionLabel("CS2 INSTALLATION FOLDER") { Left=14, Top=10 });
            txtGamePath = new DarkInput { Left=14, Top=30, Width=760, Height=34, Text=AppState.Settings.GamePath };
            var btnBrowse = new FlatBtn("Browse", Color.FromArgb(50,50,75)) { Left=784, Top=30, Width=120, Height=34, Font=Theme.Small };
            btnBrowse.Click += (s,e) =>
            {
                using (var d = new FolderBrowserDialog { Description="پوشه Counter-Strike Global Offensive رو انتخاب کن" })
                    if (d.ShowDialog() == DialogResult.OK) txtGamePath.Text = d.SelectedPath;
            };
            cPath.Controls.AddRange(new Control[]{ txtGamePath, btnBrowse });

            // نام بازیکن
            var cPlayer = new CardBox(Theme.Blue) { Left=30, Top=180, Width=550, Height=90 };
            cPlayer.Controls.Add(new SectionLabel("PLAYER NAME IN CS2") { Left=14, Top=10 });
            txtPlayerName = new DarkInput { Left=14, Top=30, Width=340, Height=34, Text=AppState.Settings.PlayerName };
            cPlayer.Controls.Add(new Label { Text="تا Player 11 به ترتیب اتصال تغییر میکنه", ForeColor=Theme.TextDim, Font=Theme.Small, Left=370, Top=38, AutoSize=true });
            cPlayer.Controls.Add(txtPlayerName);

            // NoSteam
            var cSteam = new CardBox { Left=600, Top=180, Width=380, Height=90 };
            cSteam.Controls.Add(new SectionLabel("NOSTEAM MODE") { Left=14, Top=10 });
            chkNoSteam = new CheckBox { Text="بدون Steam اجرا شود", Left=14, Top=34, AutoSize=true, ForeColor=Theme.TextPri, Font=Theme.Body, Checked=AppState.Settings.NoSteam };
            cSteam.Controls.Add(chkNoSteam);

            // RCON پیش‌فرض
            var cRcon = new CardBox(Theme.Blue) { Left=30, Top=285, Width=550, Height=130 };
            cRcon.Controls.Add(new SectionLabel("DEFAULT RCON SETTINGS") { Left=14, Top=10 });
            cRcon.Controls.Add(new FieldLabel("IP") { Left=14, Top=30 });
            var txtRIP = new DarkInput { Left=14, Top=48, Width=160, Height=30, Text=AppState.Settings.RconIP };
            cRcon.Controls.Add(new FieldLabel("Port") { Left=186, Top=30 });
            var txtRPort = new DarkInput { Left=186, Top=48, Width=80, Height=30, Text=AppState.Settings.RconPort.ToString() };
            cRcon.Controls.Add(new FieldLabel("Password") { Left=278, Top=30 });
            var txtRPass = new DarkInput { Left=278, Top=48, Width=200, Height=30, Text=AppState.Settings.RconPass, PasswordChar='●' };
            cRcon.Controls.AddRange(new Control[]{ txtRIP, txtRPort, txtRPass });

            // دکمه ذخیره
            var btnSave = new FlatBtn("💾  ذخیره همه تنظیمات", Theme.Green) { Left=30, Top=435, Width=260, Height=50, Font=new Font("Segoe UI",13f,FontStyle.Bold) };
            btnSave.ForeColor = Color.Black;
            btnSave.Click += (s,e) =>
            {
                AppState.Settings.GamePath    = txtGamePath.Text;
                AppState.Settings.PlayerName  = txtPlayerName.Text;
                AppState.Settings.NoSteam     = chkNoSteam.Checked;
                AppState.Settings.RconIP      = txtRIP.Text;
                int.TryParse(txtRPort.Text, out AppState.Settings.RconPort);
                AppState.Settings.RconPass    = txtRPass.Text;
                AppState.Settings.Save();
                lblSaved.Visible = true;
                var t = new System.Windows.Forms.Timer { Interval=2500 };
                t.Tick += (ts,te) => { lblSaved.Visible=false; t.Stop(); };
                t.Start();
                AppState.Log("✓ تنظیمات ذخیره شد");
            };
            lblSaved = new Label { Text="✓  ذخیره شد!", ForeColor=Theme.Green, Font=new Font("Segoe UI",13f,FontStyle.Bold), Left=305, Top=447, AutoSize=true, Visible=false };

            // اطلاعات
            var cAbout = new CardBox(Theme.Purple) { Left=30, Top=505, Width=400, Height=110 };
            cAbout.Controls.Add(new SectionLabel("ABOUT") { Left=14, Top=10 });
            cAbout.Controls.Add(new Label { Text="Arena GameNet Pro", Font=new Font("Segoe UI",14f,FontStyle.Bold), ForeColor=Theme.TextPri, Left=14, Top=28, AutoSize=true });
            cAbout.Controls.Add(new Label { Text="v2.0  —  Professional LAN Launcher for CS2", Font=Theme.Small, ForeColor=Theme.TextSec, Left=14, Top=58, AutoSize=true });
            cAbout.Controls.Add(new Label { Text="No Steam Required  •  LAN Only  •  RCON Support", Font=Theme.Small, ForeColor=Theme.TextDim, Left=14, Top=78, AutoSize=true });

            Controls.AddRange(new Control[]{ cPath, cPlayer, cSteam, cRcon, btnSave, lblSaved, cAbout });
        }
    }
}
