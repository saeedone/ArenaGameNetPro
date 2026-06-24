namespace ArenaGameNet
{
    public static class LanguageManager
    {
        public static bool IsPersian { get; set; } = true;

        public static string Get(string persian, string english)
        {
            return IsPersian ? persian : english;
        }
    }
}