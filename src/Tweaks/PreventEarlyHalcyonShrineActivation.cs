using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace ServerSider
{
    public class PreventEarlyHalcyonShrineActivation : TweakBase
    {
        public override bool allowed => Plugin.Enabled && preventEarlyHalcyonShrineActivation.Value;
        private readonly ConfigEntry<bool> preventEarlyHalcyonShrineActivation;

        internal PreventEarlyHalcyonShrineActivation(ConfigFile config)
        {
            preventEarlyHalcyonShrineActivation = config.Bind<bool>("Tweaks", nameof(preventEarlyHalcyonShrineActivation), true,
                "Disable the \"Pray to Halcyon Shrine\" prompt to prevent activating the shrine before it is fully charged.");
        }

        protected override void Hook()
        {
            IL.RoR2.GoldSiphonNearbyBodyController.DrainGold += GoldSiphonNearbyBodyController_DrainGold;

            Plugin.Logger.LogDebug($"{nameof(PreventEarlyHalcyonShrineActivation)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            IL.RoR2.GoldSiphonNearbyBodyController.DrainGold -= GoldSiphonNearbyBodyController_DrainGold;

            Plugin.Logger.LogDebug($"{nameof(PreventEarlyHalcyonShrineActivation)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static void GoldSiphonNearbyBodyController_DrainGold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {
                x => x.MatchLdarg(0),                                                                                                   // IL_02b9: ldarg.0
                x => x.MatchLdfld(typeof(GoldSiphonNearbyBodyController), nameof(GoldSiphonNearbyBodyController.purchaseInteraction)),  // IL_02ba: ldfld class RoR2.PurchaseInteraction RoR2.GoldSiphonNearbyBodyController::purchaseInteraction
                x => x.MatchLdcI4(1),                                                                                                   // IL_02bf: ldc.i4.1
                x => x.MatchCallOrCallvirt<PurchaseInteraction>(nameof(PurchaseInteraction.SetAvailable)),                              // IL_02c0: callvirt instance void RoR2.PurchaseInteraction::SetAvailable(bool)
            };

            if (c.TryGotoNext(match)) {
                c.RemoveRange(match.Length); // :p
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(PreventEarlyHalcyonShrineActivation)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
