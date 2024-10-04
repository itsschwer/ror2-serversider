using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace ServerSider
{
    public class TeleportOutOfBoundsPickups : TweakBase
    {
        public override bool allowed => Plugin.Enabled && teleportOutOfBoundsPickups.Value;
        private readonly ConfigEntry<bool> teleportOutOfBoundsPickups;

        private bool vanillaIgnoreLayerCollision;
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
            vanillaIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(LayerMask.NameToLayer("CollideWithCharacterHullOnly"), PickupDropletController.pickupDropletPrefab.layer);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("CollideWithCharacterHullOnly"), PickupDropletController.pickupDropletPrefab.layer, false);

            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
            On.RoR2.PickupDropletController.Start += PickupDropletController_Start;
            On.RoR2.GenericPickupController.Start += GenericPickupController_Start;

            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("CollideWithCharacterHullOnly"), PickupDropletController.pickupDropletPrefab.layer, vanillaIgnoreLayerCollision);

            On.RoR2.MapZone.TryZoneStart -= MapZone_TryZoneStart;
            On.RoR2.PickupDropletController.Start -= PickupDropletController_Start;
            On.RoR2.GenericPickupController.Start -= GenericPickupController_Start;

            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===============================

        private void GenericPickupController_Start(On.RoR2.GenericPickupController.orig_Start orig, GenericPickupController self)
        {
            orig(self);
            self.gameObject.AddComponent<OutOfBoundsPickupHelper>();
        }

        private void PickupDropletController_Start(On.RoR2.PickupDropletController.orig_Start orig, PickupDropletController self)
        {
            orig(self);
            self.gameObject.AddComponent<OutOfBoundsPickupHelper>();
        }

        private static void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            orig(self, other);

            if (self.zoneType == MapZone.ZoneType.OutOfBounds) {
                // Try teleport to spawn location
                if (other.TryGetComponent<OutOfBoundsPickupHelper>(out var helper)) {
                    Vector3 position = helper.origin;
                    TeleportHelper.TeleportGameObject(other.gameObject, position);
                    if (other.attachedRigidbody) {
                        other.attachedRigidbody.velocity = Vector3.zero;
                    }
                    Object.Destroy(helper);
                    Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Teleported pickup to {position} [entered {self.gameObject.name}].");
                }
                // Fallback to nearest node
                else {
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
                            Vector3 position = target.transform.position + (Vector3.up * other.bounds.extents.y);
                            TeleportHelper.TeleportGameObject(other.gameObject, position);
                            if (other.attachedRigidbody) {
                                other.attachedRigidbody.velocity = Vector3.zero;
                            }
                            Object.Destroy(target);
                            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Teleported pickup to {position} [entered {self.gameObject.name}] {{ {nameof(hasPickupDropletController)}: {hasPickupDropletController}, {nameof(hasGenericPickupController)}: {hasGenericPickupController} }}.");
                        } else Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Failed to find a node to teleport to [entered {self.gameObject.name}].");
                    }
                }
            }
        }

        private class OutOfBoundsPickupHelper : MonoBehaviour
        {
            public Vector3 origin { get; private set; }

            private void OnEnable()
            {
                origin = transform.position;
            }
        }
    }
}
