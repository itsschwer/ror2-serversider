using BepInEx.Configuration;

namespace ServerSider
{
    public sealed class Config
    {
        // Tweaks
        private readonly ConfigEntry<bool> rescueShipPortal;
        private readonly ConfigEntry<bool> voidFieldFogAltStart;
        private readonly ConfigEntry<bool> friendlyFireHeals;
        private readonly ConfigEntry<float> friendlyFireHealsFactor;
        // Accessors
        public bool RescueShipPortal => rescueShipPortal.Value;
        public bool VoidFieldFogAltStart => voidFieldFogAltStart.Value;
        public bool FriendlyFireHeals => friendlyFireHeals.Value;
        public float FriendlyFireHealsFactor => friendlyFireHealsFactor.Value;

        public Config(ConfigFile config)
        {
            const string Tweaks = "Tweaks";
            rescueShipPortal = config.Bind<bool>(Tweaks, nameof(rescueShipPortal), true,
                "Spawn a portal in the Rescue Ship to allow looping after defeating Mithrix.");
            voidFieldFogAltStart = config.Bind<bool>(Tweaks, nameof(voidFieldFogAltStart), false,
                "Change the Void Fields fog to only become active once a Cell Vent has been activated.");
            friendlyFireHeals = config.Bind<bool>(Tweaks, nameof(friendlyFireHeals), false,
                "TBA");
            friendlyFireHealsFactor = config.Bind<float>(Tweaks, nameof(friendlyFireHealsFactor), 0.2f,
                "TBA");
        }
    }
}
