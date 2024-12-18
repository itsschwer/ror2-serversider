﻿using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using System.Text;

namespace ServerSider
{
    public class VoidPickupConfirmAll : TweakBase
    {
        public override bool allowed => Plugin.Enabled && voidPickupConfirmAll.Value;
        private readonly ConfigEntry<bool> voidPickupConfirmAll;

        private static readonly Dictionary<ItemTier, ItemTierDef.PickupRules> originalRules = [];

        internal VoidPickupConfirmAll(ConfigFile config)
        {
            voidPickupConfirmAll = config.Bind<bool>("Tweaks", nameof(voidPickupConfirmAll), true,
                "Always require confirmation to pick up void items.");
        }

        protected override void Hook()
        {
            StringBuilder sb = new($"{nameof(VoidPickupConfirmAll)}> Hooked by {GetExecutingMethod()}");

            foreach (ItemTierDef def in ItemTierCatalog.allItemTierDefs) {
                if (def.tier == ItemTier.VoidTier1 ||
                    def.tier == ItemTier.VoidTier2 ||
                    def.tier == ItemTier.VoidTier3 ||
                    def.tier == ItemTier.VoidBoss)
                {
                    originalRules[def.tier] = def.pickupRules;
                    def.pickupRules = ItemTierDef.PickupRules.ConfirmAll;
                    sb.Append($"\n\tChanged pickup rule for item tier {def.tier} from {originalRules[def.tier]} to {def.pickupRules}");
                }
            }

            Plugin.Logger.LogDebug(sb.ToString());
        }

        protected override void Unhook()
        {
            foreach (ItemTierDef def in ItemTierCatalog.allItemTierDefs) {
                if (originalRules.TryGetValue(def.tier, out ItemTierDef.PickupRules originalRule)) {
                    def.pickupRules = originalRule;
                }
            }
            originalRules.Clear();

            Plugin.Logger.LogDebug($"{nameof(VoidPickupConfirmAll)}> Unhooked by {GetExecutingMethod()}");
        }
    }
}
