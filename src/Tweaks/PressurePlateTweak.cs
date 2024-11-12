using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace ServerSider
{
    public class PressurePlateTweak : TweakBase
    {
        public override bool allowed => pressurePlateGracePeriod.Value != 0;
        private static ConfigEntry<float> pressurePlateGracePeriod;

        internal PressurePlateTweak(ConfigFile config)
        {
            pressurePlateGracePeriod = config.Bind<float>("Tweaks", nameof(pressurePlateGracePeriod), 30f,
                "The length of time (in seconds) that a pressure plate will remain pressed after being activated.\nZero disables time functionality (reverts to vanilla behaviour). Negative values prevent pressure plates from releasing once activated.");
        }

        protected override void Hook()
        {
            On.RoR2.PressurePlateController.SetSwitch += PressurePlateController_SetSwitch;
            On.RoR2.PressurePlateController.FixedUpdate += PressurePlateController_FixedUpdate;

            Plugin.Logger.LogDebug($"{nameof(PressurePlateTweak)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.PressurePlateController.SetSwitch -= PressurePlateController_SetSwitch;
            On.RoR2.PressurePlateController.FixedUpdate -= PressurePlateController_FixedUpdate;
            platePressTimestamps.Clear();

            Plugin.Logger.LogDebug($"{nameof(PressurePlateTweak)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static readonly Dictionary<PressurePlateController, float> platePressTimestamps = new(2);

        private static void PressurePlateController_SetSwitch(On.RoR2.PressurePlateController.orig_SetSwitch orig, RoR2.PressurePlateController self, bool switchIsDown)
        {
            if (switchIsDown) {
                if (switchIsDown != self.switchDown) {
                    string message = (Random.value <= 0.2) ? "Press your plate!" : "A pressure plate is pressed..";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</style>" });
                }

                orig(self, switchIsDown);
                platePressTimestamps[self] = Time.time;
            }
            else if (pressurePlateGracePeriod.Value > 0 && !platePressTimestamps.ContainsKey(self)) {
                if (switchIsDown != self.switchDown) {
                    const string message = "A pressure plate releases...";
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "<style=cEvent>" + message + "</style>" });
                }

                orig(self, switchIsDown);
            }
        }

        private void PressurePlateController_FixedUpdate(On.RoR2.PressurePlateController.orig_FixedUpdate orig, PressurePlateController self)
        {
            orig(self);
            if (!self.enableOverlapSphere) return;

            if (platePressTimestamps.TryGetValue(self, out float initialTime)) {
                if ((Time.time - initialTime) >= pressurePlateGracePeriod.Value) {
                    platePressTimestamps.Remove(self);
                }
            }

#if DEBUG
            if (self.switchDown) {
                string identifier = (self != null) ? $"[{self.name}]" : "[no longer exists?]";
                float time = platePressTimestamps.TryGetValue(self, out float debugTime) ? Time.time - debugTime : -1;
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cIsUtility>{identifier} active {time}s ({platePressTimestamps.Count} active)</style>" });
            }
#endif
        }
    }
}
