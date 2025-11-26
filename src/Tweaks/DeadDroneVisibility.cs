using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace ServerSider
{
    public class DeadDroneVisibility : TweakBase
    {
        public override bool allowed => Plugin.Enabled && deadDroneVisibility.Value;
        private readonly ConfigEntry<bool> deadDroneVisibility;

        internal DeadDroneVisibility(ConfigFile config)
        {
            deadDroneVisibility = config.Bind<bool>("Tweaks", nameof(deadDroneVisibility), true,
                "TODO");
        }

        protected override void Hook()
        {
            IL.EntityStates.Drone.DeathState.OnImpactServer += Drone_DeathState_OnImpactServer;

            Plugin.Logger.LogDebug($"{nameof(DeadDroneVisibility)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            IL.EntityStates.Drone.DeathState.OnImpactServer -= Drone_DeathState_OnImpactServer;

            Plugin.Logger.LogDebug($"{nameof(DeadDroneVisibility)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static void Drone_DeathState_OnImpactServer(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {
                // TODO
            };

            if (c.TryGotoNext(match)) {
                // TODO
#if DEBUG || true
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(DeadDroneVisibility)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
