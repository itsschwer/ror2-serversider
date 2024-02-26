using RoR2;

namespace ServerSider
{
    public static class VoidFieldFogTweak
    {
        private static bool _hooked = false;

        public static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.RoR2.ArenaMissionController.OnStartServer += ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound += ArenaMissionController_BeginRound;

            Log.Debug($"{nameof(VoidFieldFogTweak)}> Hooked by {Log.GetExecutingMethod()}");
        }

        public static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.RoR2.ArenaMissionController.OnStartServer -= ArenaMissionController_OnStartServer;
            On.RoR2.ArenaMissionController.BeginRound -= ArenaMissionController_BeginRound;

            Log.Debug($"{nameof(VoidFieldFogTweak)}> Unhooked by {Log.GetExecutingMethod()}");
        }

        public static void Rehook(bool condition)
        {
            Unhook();
            if (condition) Hook();

            Log.Debug($"{nameof(VoidFieldFogTweak)}> Rehooked by {Log.GetExecutingMethod()}");
        }

        public static void ManageHook() => Rehook(Plugin.Enabled && Plugin.Config.VoidFieldFogAltStart);


        // Functionality ===================================

        private static void ArenaMissionController_OnStartServer(On.RoR2.ArenaMissionController.orig_OnStartServer orig, ArenaMissionController self)
        {
            orig(self);
            self.SetFogActive(false);
        }

        private static void ArenaMissionController_BeginRound(On.RoR2.ArenaMissionController.orig_BeginRound orig, ArenaMissionController self)
        {
            orig(self);
            if (self.currentRound == 1) self.SetFogActive(true);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Member Access", "Publicizer001:Accessing a member that was not originally public")]
        private static void SetFogActive(this ArenaMissionController controller, bool value)
        {
#if DEBUG
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>[Arena Fog] {(value ? "active" : "inactive")}</style>" });
#endif
            controller.fogDamageInstance?.SetActive(value);
            controller.clearedEffect.SetActive(!value);
        }
    }
}
