// Give up:
//  - breaks Railgunner Concussion Device
//  - applies DOTs (that heal)
#if FRIENDLYFIREHEALS
using RoR2;
using UnityEngine.Networking;

namespace ServerSider
{
    public static class FriendlyFireHeals
    {
        private static bool _hooked = false;

        public static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.RoR2.FriendlyFireManager.ShouldDirectHitProceed += FriendlyFireManager_ShouldDirectHitProceed;
            On.RoR2.FriendlyFireManager.ShouldSplashHitProceed += FriendlyFireManager_ShouldSplashHitProceed;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;

            Plugin.Logger.LogDebug($"{nameof(FriendlyFireHeals)}> Hooked by {Plugin.GetExecutingMethod()}");
        }

        public static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            On.RoR2.FriendlyFireManager.ShouldDirectHitProceed -= FriendlyFireManager_ShouldDirectHitProceed;
            On.RoR2.FriendlyFireManager.ShouldSplashHitProceed -= FriendlyFireManager_ShouldSplashHitProceed;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;

            Plugin.Logger.LogDebug($"{nameof(FriendlyFireHeals)}> Unhooked by {Plugin.GetExecutingMethod()}");
        }

        public static void Rehook(bool condition)
        {
            Unhook();
            if (condition) Hook();

            Plugin.Logger.LogDebug($"{nameof(FriendlyFireHeals)}> Rehooked by {Plugin.GetExecutingMethod()}");
        }

        public static void ManageHook() => Rehook(Plugin.Enabled && Plugin.Config.FriendlyFireHeals);

        // Functionality ===================================

        private static bool FriendlyFireManager_ShouldDirectHitProceed(On.RoR2.FriendlyFireManager.orig_ShouldDirectHitProceed orig, HealthComponent victim, TeamIndex attackerTeamIndex)
        {
            if (attackerTeamIndex == TeamIndex.Player && victim.body.teamComponent.teamIndex == attackerTeamIndex) return true;
            else return orig(victim, attackerTeamIndex);
        }

        private static bool FriendlyFireManager_ShouldSplashHitProceed(On.RoR2.FriendlyFireManager.orig_ShouldSplashHitProceed orig, HealthComponent victim, TeamIndex attackerTeamIndex)
        {
            if (attackerTeamIndex == TeamIndex.Player && victim.body.teamComponent.teamIndex == attackerTeamIndex) return true;
            else return orig(victim, attackerTeamIndex);
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (!NetworkServer.active) {
                UnityEngine.Debug.LogWarning("[Server] function 'System.Void RoR2.HealthComponent::TakeDamage(RoR2.DamageInfo)' called on client");
            }
            else {
                if (self.body.teamComponent.teamIndex == TeamIndex.Player) {
                    TeamComponent attackerTeam = damageInfo.attacker?.GetComponent<TeamComponent>();
                    if (attackerTeam != null) {
                        if (attackerTeam.teamIndex == TeamIndex.Player) {
                            float healAmount = damageInfo.damage * Plugin.Config.FriendlyFireHealsFactor;
                            self.Heal(healAmount, default);
#if DEBUG || true
                            Plugin.Logger.LogDebug($"{nameof(FriendlyFireHeals)}> Healed {healAmount} ({self.body.GetDisplayName()} <- {damageInfo.attacker.GetComponent<CharacterBody>()?.GetDisplayName()})");
                            Plugin.Logger.LogDebug(new System.Diagnostics.StackTrace(true).ToString());
#endif
                            return;
                        }
                    }
                }

                orig(self, damageInfo);
            }
        }
    }
}
#endif
