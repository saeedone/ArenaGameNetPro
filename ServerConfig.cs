namespace ArenaGameNet
{
    public class ServerConfig
    {
        public string ServerName { get; set; } = "Arena Server";
        public string Map { get; set; } = "de_dust2";
        public int MaxPlayers { get; set; } = 10;
        public string Password { get; set; } = "";
        public string GameMode { get; set; } = "Casual";
        public int BotCount { get; set; } = 0;
        public int BotDifficulty { get; set; } = 1;
        public bool SvCheats { get; set; } = false;

        // اسکین ورکشاپ
        public bool UseWorkshopSkins { get; set; } = false;
        public string WorkshopCollection { get; set; } = "";

        // اسکین نایف (جداگانه)
        public bool UseKnifeSkins { get; set; } = false;
        public string KnifeCollection { get; set; } = "";

        // اسکین دستکش (Gloves)
        public bool UseGloveSkins { get; set; } = false;
        public string GloveCollection { get; set; } = "";

        // اسکین لباس و مدل بازیکن (T & CT Player Models)
        public bool UsePlayerModels { get; set; } = false;
        public string PlayerModelCollection { get; set; } = "";

        // دانلود خودکار اسکین
        public bool AutoDownloadSkins { get; set; } = false;
        public string FastDLUrl { get; set; } = "";
    }
}