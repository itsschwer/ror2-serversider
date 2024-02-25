using BepInEx.Configuration;

namespace ServerSider
{
    public sealed class Config
    {
        // Tweaks
        private readonly ConfigEntry<bool> rescueShipPortal;
        private readonly ConfigEntry<bool> voidFieldFogAltStart;
        // Accessors
        public bool RescueShipPortal => rescueShipPortal.Value;
        public bool VoidFieldFogAltStart => voidFieldFogAltStart.Value;

        public Config(ConfigFile config)
        {
            const string Tweaks = "Tweaks";
            rescueShipPortal = config.Bind<bool>(Tweaks, nameof(rescueShipPortal), false,
                "Spawn a portal in the Rescue Ship to continue runs where Mithrix is defeated.");
            voidFieldFogAltStart = config.Bind<bool>(Tweaks, nameof(voidFieldFogAltStart), false,
                "Change the Void Fields fog to only become active once a Cell Vent has been activated.");
        }
    }
}
