using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using UnityEngine;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace ServerSider
{
    public class QuitToLobbyButton : TweakBase
    {
        public override bool allowed => Plugin.Enabled && quitToLobbyButton.Value;
        private readonly ConfigEntry<bool> quitToLobbyButton;

        private const string commandName = $"{Plugin.Author}_run_end";

        internal QuitToLobbyButton(ConfigFile config)
        {
            quitToLobbyButton = config.Bind<bool>("Tweaks", nameof(quitToLobbyButton), true,
                "Add a \"Quit to Lobby\" button to the Run pause menu.\n\nUseful in multiplayer for changing characters/survivors, difficulty, or artifacts without disconnecting everyone from the lobby.");
        }

        protected override void Hook()
        {
            On.RoR2.UI.PauseScreenController.Awake += PauseScreenController_Awake;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.UI.PauseScreenController.Awake -= PauseScreenController_Awake;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===============================

        private static void PauseScreenController_Awake(On.RoR2.UI.PauseScreenController.orig_Awake orig, PauseScreenController self)
        {
            orig(self);

            if (Run.instance != null) {
                PauseScreenController_Awake(self);
            }
        }

        private static void PauseScreenController_Awake(PauseScreenController __instance)
        {
            const string label = "Quit to Lobby"; // Should capitalise "to" to match existing buttons, but I prefer title case
            GameObject obj = Object.Instantiate(__instance.exitGameButton, __instance.exitGameButton.transform.parent);
            Object.DestroyImmediate(obj.GetComponent<LanguageTextMeshController>()); // Will override text (e.g. when returning from a submenu)
            obj.name = $"GenericMenuButton ({label})";
            obj.GetComponentInChildren<HGTextMeshProUGUI>().text = label;
            HGButton btn = obj.GetComponent<HGButton>();
            // [0] SubmitCmd | OptionsPanel(JUICED)(RoR2.ConsoleFunctions)
            //     quit_confirmed_command "quit"
            btn.onClick.m_PersistentCalls.GetListener(0).arguments.stringArgument = $"quit_confirmed_command \"{commandName}\"";
            // quitToLobbyClicked = true; // need to set thsi when the action is confirmed...
        }

        [ConCommand(commandName = commandName, flags = ConVarFlags.SenderMustBeServer)]
        private static void CCRunEndUnpause(ConCommandArgs args)
        {
            Console.instance.RunCmd(LocalUserManager.GetFirstLocalUser(), "run_end", []);
            if (PauseScreenController.instancesList.Count == 0) return;

            Object.Destroy(PauseScreenController.instancesList[0].gameObject);
        }
    }
}
