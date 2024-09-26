using BepInEx.Configuration;
using RoR2;

namespace ServerSider
{
    public class VoidFieldFogTweak : TweakBase
    {
        public override bool allowed => Plugin.Enabled && voidFieldFogAltStart.Value;
        private readonly ConfigEntry<bool> voidFieldFogAltStart;

        internal VoidFieldFogTweak(ConfigFile config)
        {
            voidFieldFogAltStart = config.Bind<bool>("Tweaks", nameof(voidFieldFogAltStart), false,
                "Change the Void Fields fog to only become active once a Cell Vent has been activated.");
        }

        protected override void Hook()
        {
            On.RoR2.ArenaMissionController.OnStartServer += ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound += ArenaMissionController_BeginRound;

            Plugin.Logger.LogDebug($"{nameof(VoidFieldFogTweak)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.ArenaMissionController.OnStartServer -= ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound -= ArenaMissionController_BeginRound;

            Plugin.Logger.LogDebug($"{nameof(VoidFieldFogTweak)}> Unhooked by {GetExecutingMethod()}");
        }


        // Functionality ===================================

        private static void ArenaMissionController_OnStartServer(On.RoR2.ArenaMissionController.orig_OnStartServer orig, ArenaMissionController self)
        {
            orig(self);
            SetFogActive(self, false);
        }

        private static void ArenaMissionController_BeginRound(On.RoR2.ArenaMissionController.orig_BeginRound orig, ArenaMissionController self)
        {
            orig(self);
            if (self.currentRound == 1) SetFogActive(self, true);
        }

        private static void SetFogActive(ArenaMissionController controller, bool value)
        {
#if DEBUG
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>[Arena Fog] {(value ? "active" : "inactive")}</style>" });
#endif
            controller.fogDamageInstance?.SetActive(value);
            controller.clearedEffect.SetActive(!value);
        }
    }
}
