using ArenaGameNet.Models;

namespace ArenaGameNet.Services
{
    public class SkinService
    {
        public string BuildSkinArguments(SkinConfig config)
        {
            string args = "";

            if (config.AutoDownload)
            {
                args += " +sv_allowdownload 1 +sv_allowupload 1 +cl_allowdownload 1 +cl_allowupload 1";
            }

            if (config.UseGunSkins && !string.IsNullOrEmpty(config.GunCollection))
                args += $" +host_workshop_collection {config.GunCollection}";

            if (config.UseKnifeSkins && !string.IsNullOrEmpty(config.KnifeCollection))
                args += $" +host_workshop_collection {config.KnifeCollection}";

            if (config.UseGloveSkins && !string.IsNullOrEmpty(config.GloveCollection))
                args += $" +host_workshop_collection {config.GloveCollection}";

            if (config.UsePlayerModels && !string.IsNullOrEmpty(config.PlayerModelCollection))
                args += $" +host_workshop_collection {config.PlayerModelCollection}";

            if (!string.IsNullOrEmpty(config.FastDLUrl))
                args += $" +sv_downloadurl \"{config.FastDLUrl}\"";

            return args;
        }
    }
}