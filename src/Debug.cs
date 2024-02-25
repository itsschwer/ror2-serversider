#if DEBUG
using RoR2;
using UnityEngine;

namespace ServerSider
{
    internal static class Debug
    {
        internal static void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8)) DisableEnemySpawns();
            else if (Input.GetKeyDown(KeyCode.F9)) TestRescueShipLoopPortal(LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetCurrentBody());
            else if (Input.GetKeyUp(KeyCode.F10)) ForceEscapeSequence();
            else if (Input.GetKeyUp(KeyCode.F11)) GiveMobilityItems(LocalUserManager.GetFirstLocalUser().currentNetworkUser.master);
            else if (Input.GetKeyUp(KeyCode.F12)) GiveCombatItems(LocalUserManager.GetFirstLocalUser().currentNetworkUser.master);

            else if (Input.GetKeyUp(KeyCode.F5)) ForceStage("moon2");
            else if (Input.GetKeyUp(KeyCode.F6)) ForceStage("arena");
        }

        private static void DisableEnemySpawns()
        {
            bool wasDisabled = CombatDirector.cvDirectorCombatDisable.GetString() != "0";
            CombatDirector.cvDirectorCombatDisable.SetBool(!wasDisabled);
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = $"<style=cWorldEvent>Enemy spawns {(wasDisabled ? "enabled" : "disabled")}</style>" });
        }

        private static void ForceStage(string sceneBaseName)
        {
            // Run.instance.AdvanceStage(scene);
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            Run.instance.GenerateStageRNG();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
            UnityEngine.Networking.NetworkManager.singleton.ServerChangeScene(sceneBaseName);
        }

        // RescueShipLoopPortal ============================

        private static void TestRescueShipLoopPortal(CharacterBody target)
        {
            InputBankTest input = target?.inputBank;
            if (input == null) return;

            RescueShipLoopPortal.InstantiatePortal(target.footPosition, Quaternion.LookRotation(-input.aimDirection));
        }

        private static void ForceEscapeSequence()
        {
            EscapeSequenceController esc = Object.FindObjectOfType<EscapeSequenceController>();
            esc?.onEnterMainEscapeSequence?.Invoke();
        }

        static void GiveMobilityItems(CharacterMaster target)
        {
            if (target?.inventory == null) return;

            target.inventory.GiveItem(RoR2Content.Items.SprintBonus, 15);
            target.inventory.GiveItem(RoR2Content.Items.Hoof, 10);
        }

        static void GiveCombatItems(CharacterMaster target)
        {
            if (target?.inventory == null) return;

            target.inventory.GiveItem(RoR2Content.Items.SprintWisp, 500);
            target.inventory.GiveItem(RoR2Content.Items.Bear, 70);
            target.inventory.GiveItem(RoR2Content.Items.LunarBadLuck, 11);
            target.inventory.GiveItem(RoR2Content.Items.Clover, 11);
            target.inventory.GiveItem(RoR2Content.Items.FallBoots, 100);
        }
    }
}
#endif
