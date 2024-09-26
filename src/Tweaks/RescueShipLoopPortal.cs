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

            if (self.inBoundsObjectiveToken == "OBJECTIVE_MOON_CHARGE_DROPSHIP")
            {
#if DEBUG
                Plugin.Logger.LogDebug($"Dropship> pos: {self.transform.position} | rot: {self.transform.rotation} | euler: {self.transform.rotation.eulerAngles} | up: {self.transform.up} | forward: {self.transform.forward} | right: {self.transform.right}");
#endif
                Vector3 position = self.transform.position;
                position += (self.transform.up * 10f);
                position += (self.transform.forward * -3.5f);
                position += (self.transform.right * -1.8f);
                Quaternion rotation = Quaternion.Inverse(self.transform.rotation) * Quaternion.Euler(self.transform.up * 90f);
                InstantiatePortal(position, rotation);
            }
        }

        internal static void InstantiatePortal(Vector3 position, Quaternion rotation)
        {
            InteractableSpawnCard isc = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/iscInfiniteTowerPortal.asset").WaitForCompletion();

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
