using BepInEx.Configuration;

namespace ServerSider
{
    public sealed class Config
    {
        private readonly ConfigFile file;
        internal void Reload() { Plugin.Logger.LogDebug($"Reloading {file.ConfigFilePath.Substring(file.ConfigFilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1)}"); file.Reload(); }


        // Tweaks
        private readonly ConfigEntry<bool> rescueShipPortal;
        private readonly ConfigEntry<bool> voidFieldFogAltStart;
        private readonly ConfigEntry<bool> chanceDollMessage;
#if FRIENDLYFIREHEALS
        private readonly ConfigEntry<bool> friendlyFireHeals;
        private readonly ConfigEntry<float> friendlyFireHealsFactor;
#endif
        // Accessors
        public bool RescueShipPortal => rescueShipPortal.Value;
        public bool VoidFieldFogAltStart => voidFieldFogAltStart.Value;
        public bool ChanceDollMessage => chanceDollMessage.Value;
#if FRIENDLYFIREHEALS
        public bool FriendlyFireHeals => friendlyFireHeals.Value;
        public float FriendlyFireHealsFactor => friendlyFireHealsFactor.Value;
#endif

        public Config(ConfigFile config)
        {
            file = config;

            const string Tweaks = "Tweaks";
            rescueShipPortal = config.Bind<bool>(Tweaks, nameof(rescueShipPortal), true,
                "Spawn a portal in the Rescue Ship to allow looping after defeating Mithrix.");
            voidFieldFogAltStart = config.Bind<bool>(Tweaks, nameof(voidFieldFogAltStart), false,
                "Change the Void Fields fog to only become active once a Cell Vent has been activated.");
            chanceDollMessage = config.Bind<bool>(Tweaks, nameof(chanceDollMessage), true,
                "Reword the Shrine of Chance success message to indicate if a Chance Doll affected the reward.");
#if FRIENDLYFIREHEALS
            friendlyFireHeals = config.Bind<bool>(Tweaks, nameof(friendlyFireHeals), false,
                "TBA");
            friendlyFireHealsFactor = config.Bind<float>(Tweaks, nameof(friendlyFireHealsFactor), 0.2f,
                "TBA");
#endif
        }
    }
}
