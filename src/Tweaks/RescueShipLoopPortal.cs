using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace ServerSider
{
    public class RescueShipLoopPortal : TweakBase
    {
        public override bool allowed => Plugin.Enabled && rescueShipPortal.Value;
        private readonly ConfigEntry<bool> rescueShipPortal;

        internal RescueShipLoopPortal(ConfigFile config)
        {
            rescueShipPortal = config.Bind<bool>("Tweaks", nameof(rescueShipPortal), true,
                "Spawn a portal in the Rescue Ship to allow looping after defeating Mithrix.");
        }

        protected override void Hook()
        {
            On.RoR2.HoldoutZoneController.Start += HoldoutZoneController_Start;

            Plugin.Logger.LogDebug($"{nameof(RescueShipLoopPortal)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.HoldoutZoneController.Start -= HoldoutZoneController_Start;

            Plugin.Logger.LogDebug($"{nameof(RescueShipLoopPortal)}> Unhooked by {GetExecutingMethod()}");
        }


        // Functionality ===================================

        private static void HoldoutZoneController_Start(On.RoR2.HoldoutZoneController.orig_Start orig, HoldoutZoneController self)
        {
            orig(self);

            if (self.inBoundsObjectiveToken == "OBJECTIVE_MOON_CHARGE_DROPSHIP") {
                Transform target = null;
                try {
                    // Moon2DropshipZone/HoldoutZoneContainer/HoldoutZone
                    target = self.transform.parent.parent.Find("RescueshipMoon/escapeship/DropshipMesh");
                }
                catch (System.NullReferenceException e) {
                    Plugin.Logger.LogError($"{nameof(RescueShipLoopPortal)}> {e}");
                }
                if (target == null) { Plugin.Logger.LogError($"{nameof(RescueShipLoopPortal)}> Failed to find \"Moon2DropshipZone/RescueshipMoon/escapeship/DropshipMesh\""); return; }

                Vector3 position = target.position;
                position += (target.up * 7f);
                position += (target.forward * 14f);
                InstantiatePortal(position, Quaternion.LookRotation(target.forward, -target.up)); // DropshipMesh is rotated -89.98 on x-axis (upside-down)
#if DEBUG
                Plugin.Logger.LogDebug($"{nameof(RescueShipLoopPortal)}> offset: {position - self.transform.position} [{position - target.position}]");
#endif
            }
        }

        internal static void InstantiatePortal(Vector3 position, Quaternion rotation)
        {
            // pre-alloyed collective "RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/iscInfiniteTowerPortal.asset"
            //     alloyed collective "RoR2/DLC1/GameModes/InfiniteTowerRun/ITAssets/iscInfiniteTowerPortal.asset"
            const string key = "0fe24a82acb288346a0779181fc66c39";
#if DEBUG
            Plugin.Logger.LogDebug($"iscInfiniteTowerPortal.asset: {RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_GameModes_InfiniteTowerRun_InfiniteTowerAssets.iscInfiniteTowerPortal_asset}");
#endif
            InteractableSpawnCard isc = Addressables.LoadAssetAsync<InteractableSpawnCard>(key).WaitForCompletion();

            // RoR2.InteractableSpawnCard.Spawn() & RoR2.ArtifactTrialMissionController.SpawnExitPortalAndIdle.OnEnter()
            GameObject gameObject = Object.Instantiate(isc.prefab, position, rotation);
            gameObject.GetComponent<SceneExitController>().useRunNextStageScene = true;
#if DEBUG
            Plugin.Logger.LogDebug($"{nameof(RescueShipLoopPortal)}> pos: {position} | rot: {rotation}");
#endif
            NetworkServer.Spawn(gameObject);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<size=120%><style=cIsVoid>The Void calls...</style></size>" });
        }
    }
}
