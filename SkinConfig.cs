namespace ArenaGameNet.Models
{
    public class SkinConfig
    {
        public bool UseGunSkins { get; set; }
        public string GunCollection { get; set; }

        public bool UseKnifeSkins { get; set; }
        public string KnifeCollection { get; set; }

        public bool UseGloveSkins { get; set; }
        public string GloveCollection { get; set; }

        public bool UsePlayerModels { get; set; }
        public string PlayerModelCollection { get; set; }

        public bool AutoDownload { get; set; }
        public string FastDLUrl { get; set; }
    }
}