using System.Drawing;

namespace AGP
{
    public static class Theme
    {
        public static Color Bg        = Color.FromArgb(10, 10, 15);
        public static Color Panel     = Color.FromArgb(20, 20, 30);
        public static Color Card      = Color.FromArgb(25, 25, 38);
        public static Color Border    = Color.FromArgb(50, 50, 72);
        public static Color Accent    = Color.FromArgb(220, 38, 38);
        public static Color Green     = Color.FromArgb(34, 197, 94);
        public static Color Blue      = Color.FromArgb(59, 130, 246);
        public static Color Gold      = Color.FromArgb(234, 179, 8);
        public static Color Purple    = Color.FromArgb(139, 92, 246);
        public static Color TextPri   = Color.FromArgb(241, 241, 255);
        public static Color TextSec   = Color.FromArgb(148, 148, 180);
        public static Color TextDim   = Color.FromArgb(75, 75, 105);
        public static Color InputBg   = Color.FromArgb(15, 15, 25);
        public static Color LogBg     = Color.FromArgb(6, 6, 12);

        public static Font H1     = new Font("Segoe UI", 20f, FontStyle.Bold);
        public static Font H2     = new Font("Segoe UI", 13f, FontStyle.Bold);
        public static Font Body   = new Font("Segoe UI", 10f);
        public static Font Small  = new Font("Segoe UI", 8.5f);
        public static Font Label  = new Font("Segoe UI", 8f, FontStyle.Bold);
        public static Font Btn    = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public static Font Mono   = new Font("Consolas", 9f);
    }
}
