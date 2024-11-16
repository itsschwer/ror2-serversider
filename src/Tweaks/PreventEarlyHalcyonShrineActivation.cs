using BepInEx.Configuration;
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

        private static readonly MethodInfo HalcyoniteShrineInteractable_TrackInteractions;
        private static readonly Hook hook;

        static PreventEarlyHalcyonShrineActivation()
        {
            // const string HALCYON_SHRINE_TYPE_NAME = "RoR2.HalcyoniteShrineInteractable";
            // // RoR2.PurchaseInteraction, RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
            // string fullName = typeof(PurchaseInteraction).AssemblyQualifiedName.Replace("RoR2.PurchaseInteraction", HALCYON_SHRINE_TYPE_NAME);
            // Plugin.Logger.LogDebug(fullName);
            // HalcyoniteShrineInteractable_TrackInteractions = Type.GetType(HALCYON_SHRINE_TYPE_NAME)?.GetMethod("TrackInteractions", BindingFlags.Instance | BindingFlags.Public);
            HalcyoniteShrineInteractable_TrackInteractions = typeof(PurchaseInteraction).Assembly.GetType("RoR2.HalcyoniteShrineInteractable")?.GetMethod("TrackInteractions", BindingFlags.Instance | BindingFlags.Public);

            if (HalcyoniteShrineInteractable_TrackInteractions == null) {
                Plugin.Logger.LogWarning($"{nameof(PreventEarlyHalcyonShrineActivation)}> Cannot hook: no HalcyoniteShrineInteractable.TrackInteractions method.");
            }
            else hook = new Hook(HalcyoniteShrineInteractable_TrackInteractions, typeof(PreventEarlyHalcyonShrineActivation).GetMethod(nameof(TrackInteractions), BindingFlags.Static | BindingFlags.NonPublic), new HookConfig { ManualApply = true });
        }

        internal PreventEarlyHalcyonShrineActivation(ConfigFile config)
        {
            preventEarlyHalcyonShrineActivation = config.Bind<bool>("Tweaks", nameof(preventEarlyHalcyonShrineActivation), true,
                "Disable the Halcyon Shrines pray prompt to prevent activating the shrine before it is fully charged.");
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

        private static void TrackInteractions(Action<NetworkBehaviour> orig, NetworkBehaviour self)
        {
            orig(self);

            if (!NetworkServer.active) return;
            self.GetComponent<PurchaseInteraction>().Networkavailable = false;
            Chat.AddMessage(self.name);
        }
    }
}
