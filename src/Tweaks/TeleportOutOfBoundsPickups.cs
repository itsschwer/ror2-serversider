using BepInEx.Configuration;
using RoR2;
using System.Text;
using UnityEngine;

namespace ServerSider
{
    public class TeleportOutOfBoundsPickups : TweakBase
    {
        public override bool allowed => Plugin.Enabled && teleportOutOfBoundsPickups.Value;
        private readonly ConfigEntry<bool> teleportOutOfBoundsPickups;

        private const string mapZoneLayerName = "CollideWithCharacterHullOnly";
        private static int dropletLayer => PickupDropletController.pickupDropletPrefab.layer;

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
            StringBuilder sb = new($"{nameof(TeleportOutOfBoundsPickups)}> Hooked by {GetExecutingMethod()}");

            int mapZoneLayer = LayerMask.NameToLayer(mapZoneLayerName);
            vanillaIgnoreLayerCollision = Physics.GetIgnoreLayerCollision(mapZoneLayer, dropletLayer);
            Physics.IgnoreLayerCollision(mapZoneLayer, dropletLayer, false);
            sb.Append($"\n\tChanged ignore layer collision rule between layers {mapZoneLayer} and {dropletLayer} from {vanillaIgnoreLayerCollision} to {false}");

            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
            On.RoR2.PickupDropletController.Start += PickupDropletController_Start;
            On.RoR2.GenericPickupController.Start += GenericPickupController_Start;

            Plugin.Logger.LogDebug(sb.ToString());
        }

        protected override void Unhook()
        {
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer(mapZoneLayerName), dropletLayer, vanillaIgnoreLayerCollision);

            On.RoR2.MapZone.TryZoneStart -= MapZone_TryZoneStart;
            On.RoR2.PickupDropletController.Start -= PickupDropletController_Start;
            On.RoR2.GenericPickupController.Start -= GenericPickupController_Start;

            Plugin.Logger.LogDebug($"{nameof(TeleportOutOfBoundsPickups)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

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
                StringBuilder sb = new($"{nameof(TeleportOutOfBoundsPickups)}> ");
                // Try teleport to spawn location
                if (other.TryGetComponent<OutOfBoundsPickupHelper>(out var helper)) {
                    if (!helper.attempted) {
                        Vector3 position = helper.origin;
                        TeleportHelper.TeleportGameObject(other.gameObject, position);
                        if (other.attachedRigidbody) {
                            other.attachedRigidbody.velocity = Vector3.zero;
                        }
                        helper.attempted = true;
                        sb.AppendLine($"Teleported pickup to {position}");
                        sb.AppendLine($"\tentered {self.gameObject.name}");
                        Plugin.Logger.LogDebug(sb.ToString());
                    }
                    // Or nearest node to spawn location
                    else {
                        Vector3? position = TeleportToNearestNode(other);
                        if (position.HasValue) {
                            sb.AppendLine($"Teleported pickup to {position.Value}");
                            sb.AppendLine($"\tentered {self.gameObject.name}");
                            Plugin.Logger.LogDebug(sb.ToString());
                        }
                    }
                }
                // Fallback to nearest node to current location
                else {
                    bool hasPickupDropletController = other.TryGetComponent<PickupDropletController>(out _); // Droplet form
                    bool hasGenericPickupController = other.TryGetComponent<GenericPickupController>(out _); // Tangible form
                    bool hasPickupPickerController = other.TryGetComponent<PickupPickerController>(out _);   // Command cube form
                    string debugComponent = $"{{ {nameof(hasPickupDropletController)}: {hasPickupDropletController}, {nameof(hasGenericPickupController)}: {hasGenericPickupController}, {nameof(hasPickupPickerController)}: {hasPickupPickerController} }}";
                    if (hasPickupDropletController || hasGenericPickupController || hasPickupPickerController) {
                        Vector3? position = TeleportToNearestNode(other);
                        if (position.HasValue) {
                            sb.AppendLine($"Teleported pickup to {position.Value}");
                            sb.AppendLine($"\tentered {self.gameObject.name}");
                            sb.AppendLine($"\t{debugComponent}");
                            Plugin.Logger.LogDebug(sb.ToString());
                        }
                        else {
                            sb.AppendLine($"Failed to find a node to teleport to (from {other.transform.position})");
                            sb.AppendLine($"\tentered {self.gameObject.name}");
                            Plugin.Logger.LogDebug(sb.ToString());
                        }
                    }
                }
            }
        }

        private static Vector3? TeleportToNearestNode(Collider other)
        {
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
                return position;
            }
            return null;
        }

        private class OutOfBoundsPickupHelper : MonoBehaviour
        {
            public Vector3 origin { get; private set; }
            public bool attempted { get; internal set; }

            private void OnEnable()
            {
                origin = transform.position;
                attempted = false;
            }
        }
    }
}
