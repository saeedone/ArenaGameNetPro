using System.Drawing;

namespace ArenaGameNet
{
    public static class Theme
    {
        // رنگ‌بندی لوکس و حرفه‌ای (سبک RogStar + Modern)
        public static Color Bg         = Color.FromArgb(15, 15, 22);
        public static Color Panel      = Color.FromArgb(25, 25, 35);
        public static Color Card       = Color.FromArgb(32, 32, 45);
        public static Color Border     = Color.FromArgb(60, 60, 80);
        public static Color Accent     = Color.FromArgb(235, 55, 55);
        public static Color AccentHov  = Color.FromArgb(255, 80, 80);
        public static Color Green      = Color.FromArgb(60, 230, 130);
        public static Color Blue       = Color.FromArgb(80, 150, 255);
        public static Color Gold       = Color.FromArgb(240, 200, 70);
        public static Color TextPri    = Color.FromArgb(250, 250, 255);
        public static Color TextSec    = Color.FromArgb(180, 180, 200);
        public static Color TextDim    = Color.FromArgb(120, 120, 145);
        public static Color InputBg    = Color.FromArgb(20, 20, 30);
        public static Color LogBg      = Color.FromArgb(10, 10, 16);

        public static Font FontTitle  = new Font("Segoe UI", 18f, FontStyle.Bold);
        public static Font FontSub    = new Font("Segoe UI", 9f);
        public static Font FontBtn    = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public static Font FontMono   = new Font("Consolas", 9f);
        public static Font FontSmall  = new Font("Segoe UI", 8f);
        public static Font FontLabel  = new Font("Segoe UI", 8.5f, FontStyle.Bold);
    }
}