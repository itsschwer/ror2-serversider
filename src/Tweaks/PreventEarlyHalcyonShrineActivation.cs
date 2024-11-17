using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using RoR2;
using System;
using System.Reflection;
using UnityEngine.Networking;

namespace ServerSider
{
    public class PreventEarlyHalcyonShrineActivation : TweakBase
    {
        public override bool allowed => Plugin.Enabled && preventEarlyHalcyonShrineActivation.Value;
        private readonly ConfigEntry<bool> preventEarlyHalcyonShrineActivation;

        private static readonly Type targetType;
        private static readonly MethodInfo targetMethod;
        private static readonly ILHook hook;

        static PreventEarlyHalcyonShrineActivation()
        {
            // string-based since still wanting to maintain compatibility with pre-Seekers of the Storm
            const string typeName = "RoR2.GoldSiphonNearbyBodyController";
            const string methodName = "DrainGold";
            targetType = typeof(PurchaseInteraction).Assembly.GetType(typeName);
            targetMethod = targetType?.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (targetMethod == null) {
                Plugin.Logger.LogWarning($"{nameof(PreventEarlyHalcyonShrineActivation)}> Cannot hook: no {typeName}.{methodName} method.");
            }
            else {
                hook = new ILHook(targetMethod, GoldSiphonNearbyBodyController_DrainGold, new ILHookConfig() { ManualApply = true });
            }
        }

        internal PreventEarlyHalcyonShrineActivation(ConfigFile config)
        {
            preventEarlyHalcyonShrineActivation = config.Bind<bool>("Tweaks", nameof(preventEarlyHalcyonShrineActivation), true,
                "Disable the \"Pray to Halcyon Shrine\" prompt to prevent activating the shrine before it is fully charged.");
        }

        protected override void Hook()
        {
            if (hook == null) return;
            hook.Apply();

            Plugin.Logger.LogDebug($"{nameof(PreventEarlyHalcyonShrineActivation)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            if (hook == null) return;
            hook.Undo();

            Plugin.Logger.LogDebug($"{nameof(PreventEarlyHalcyonShrineActivation)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static void GoldSiphonNearbyBodyController_DrainGold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {
                x => x.MatchLdarg(0),                                                                      // IL_02b9: ldarg.0
                x => x.MatchLdfld(targetType, "purchaseInteraction"),                                      // IL_02ba: ldfld class RoR2.PurchaseInteraction RoR2.GoldSiphonNearbyBodyController::purchaseInteraction
                x => x.MatchLdcI4(1),                                                                      // IL_02bf: ldc.i4.1
                x => x.MatchCallOrCallvirt<PurchaseInteraction>(nameof(PurchaseInteraction.SetAvailable)), // IL_02c0: callvirt instance void RoR2.PurchaseInteraction::SetAvailable(bool)
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
