using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace ServerSider
{
    public class UnwrapCommandEssence : TweakBase
    {
        public override bool allowed => Plugin.Enabled && unwrapCommandEssence.Value;
        private readonly ConfigEntry<bool> unwrapCommandEssence;

        internal UnwrapCommandEssence(ConfigFile config)
        {
            unwrapCommandEssence = config.Bind<bool>("Tweaks", nameof(unwrapCommandEssence), true,
                "");
        }

        protected override void Hook()
        {
            IL.RoR2.PickupDropletController.CreatePickup += PickupDropletController_CreatePickup;

            Plugin.Logger.LogDebug($"{nameof(UnwrapCommandEssence)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            IL.RoR2.PickupDropletController.CreatePickup -= PickupDropletController_CreatePickup;

            Plugin.Logger.LogDebug($"{nameof(UnwrapCommandEssence)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static void PickupDropletController_CreatePickup(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            ILLabel createNormalPickup = il.DefineLabel();

            Func<Instruction, bool>[] match = {
                x => x.MatchBrfalse(out createNormalPickup),
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<PickupDropletController>(nameof(PickupDropletController.CreateCommandCube))
                // ret
            };

            if (c.TryGotoNext(match)) {
                c.Index++; // move past brfalse
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<PickupDropletController, bool>>((@this) => {
#if ALLOYED_COLLECTIVE
                    var options = PickupPickerController.GetOptionsFromPickupState(@this.createPickupInfo.pickup);
#else
                    var options = PickupPickerController.GetOptionsFromPickupIndex(@this.createPickupInfo.pickupIndex);
#endif
                    return options.Length == 1;
                });
                c.Emit(OpCodes.Brfalse, createNormalPickup);
#if DEBUG || true
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(UnwrapCommandEssence)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
