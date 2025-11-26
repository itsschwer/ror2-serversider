using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace ServerSider
{
    public class DeadDroneVisibility : TweakBase
    {
        public override bool allowed => false;// Plugin.Enabled && deadDroneVisibility.Value;
        private readonly ConfigEntry<bool> deadDroneVisibility;

        internal DeadDroneVisibility(ConfigFile config)
        {
            return;
            deadDroneVisibility = config.Bind<bool>("Tweaks", nameof(deadDroneVisibility), true,
                "TODO");
        }

        protected override void Hook()
        {
            return;
            IL.EntityStates.Drone.DeathState.OnImpactServer += Drone_DeathState_OnImpactServer;

            Plugin.Logger.LogDebug($"{nameof(DeadDroneVisibility)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            return;
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
                // need networkspawn an existing usable beam prefab?
                // or maybe networkspawn a shield wall power pedestal pyramid and steal the beam vfx?
                // (is reparenting game objects and destroying leftovers feasible (synced from server to clients)?)
#if DEBUG || true
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(DeadDroneVisibility)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
