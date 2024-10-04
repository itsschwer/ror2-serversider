using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace ServerSider
{
    public class TeleportOutOfBoundsPickups : TweakBase
    {
        public override bool allowed => Plugin.Enabled && teleportOutOfBoundsPickups.Value;
        private readonly ConfigEntry<bool> teleportOutOfBoundsPickups;

        private static SpawnCard _scTeleportHelper;
        private static SpawnCard scTeleportHelper {
            get {
                if (_scTeleportHelper == null) {
                    // Logic from RoR2.Run.FindSafeTeleportPosition
                    SpawnCard sc = ScriptableObject.CreateInstance<SpawnCard>();
                    sc.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
                    sc.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
                    sc.hullSize = HullClassification.Human;
                    _scTeleportHelper = sc;
                }
                return _scTeleportHelper;
            }
        }

        internal TeleportOutOfBoundsPickups(ConfigFile config)
        {
            teleportOutOfBoundsPickups = config.Bind<bool>("Tweaks", nameof(teleportOutOfBoundsPickups), true,
                "Teleport items that fall off the map to the nearest valid ground node.");
        }

        protected override void Hook()
        {
            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;

            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.MapZone.TryZoneStart -= MapZone_TryZoneStart;

            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===============================

        private static void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            orig(self, other);

            if (self.zoneType == MapZone.ZoneType.OutOfBounds) {
                //todo: determine if it is possible to try teleport to try teleport to the spawn location (PickupDropletController?) before defaulting to the nearest node
                bool hasPickupDropletController = other.TryGetComponent<PickupDropletController>(out _);
                bool hasGenericPickupController = other.TryGetComponent<GenericPickupController>(out _);
                if (hasPickupDropletController || hasGenericPickupController) {
                    // Logic from RoR2.Run.FindSafeTeleportPosition
                    GameObject target = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(scTeleportHelper,
                        new DirectorPlacementRule {
                            placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                            position = other.transform.position
                        }, RoR2Application.rng));
                    if (target != null) {
                        TeleportHelper.TeleportGameObject(other.gameObject, target.transform.position);
                        Object.Destroy(target);
                        Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Teleported pickup {{ {nameof(hasPickupDropletController)}: {hasPickupDropletController}, {nameof(hasGenericPickupController)}: {hasGenericPickupController} }}.");
                    } else Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Failed to find a node to teleport to.");
                }
            }
        }
    }
}
