﻿using RoR2.UI;
using UnityEngine;

namespace ServerSider
{
    internal static class QuitToLobbyButton
    {
        private static bool _hooked = false;

        public static void Hook()
        {
            if (_hooked) return;
            _hooked = true;

            On.RoR2.UI.PauseScreenController.Awake += PauseScreenController_Awake;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Hooked by {Plugin.GetExecutingMethod()}");
        }

        public static void Unhook()
        {
            if (!_hooked) return;
            _hooked = false;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Unhooked by {Plugin.GetExecutingMethod()}");
        }

        public static void Rehook(bool condition)
        {
            Unhook();
            if (condition) Hook();

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Rehooked by {Plugin.GetExecutingMethod()}");
        }

        public static void ManageHook() => Rehook(Plugin.Enabled && Plugin.Config.QuitToLobbyButton);

        // Functionality ===============================

        private static void PauseScreenController_Awake(On.RoR2.UI.PauseScreenController.orig_Awake orig, PauseScreenController self)
        {
            orig(self);
            PauseScreenController_Awake(self);
        }

        private static void PauseScreenController_Awake(PauseScreenController __instance)
        {
            HGButton[] candidates = __instance.exitGameButton.gameObject.transform.parent.GetComponentsInChildren<HGButton>();
            for (int i = 0; i < candidates.Length; i++) {
                // Look for "Quit to Menu" button
                Plugin.Logger.LogWarning(candidates[i].name);
                if (candidates[i].onClick.m_PersistentCalls.GetListener(0).arguments.stringArgument == "quit_confirmed_command \"transition_command disconnect;\"") {
                    CreateButton(candidates[i]);
                    return;
                }
            }
            Plugin.Logger.LogWarning($"{nameof(QuitToLobbyButton)}> Failed to create button (could not find base to clone).");
        }

        private static void CreateButton(HGButton cloneBase)
        {
            const string label = "Quit to Lobby"; // Should capitalise "to" to match existing buttons, but I prefer title case
            GameObject obj = Object.Instantiate(cloneBase.gameObject, cloneBase.transform.parent);
            Object.DestroyImmediate(obj.GetComponent<LanguageTextMeshController>()); // Will override text (e.g. when returning from a submenu)
            obj.name = $"GenericMenuButton ({label})";
            obj.GetComponentInChildren<HGTextMeshProUGUI>().text = label;
            HGButton btn = obj.GetComponent<HGButton>();
            // "Quit to Desktop" button (exitGameButton)
            // [0] SubmitCmd | OptionsPanel(JUICED)(RoR2.ConsoleFunctions)
            //     quit_confirmed_command "quit"
            btn.onClick.m_PersistentCalls.GetListener(0).arguments.stringArgument = "quit_confirmed_command \"run_end\"";
        }
    }
}
