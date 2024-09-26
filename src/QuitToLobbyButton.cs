using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.UI;
using UnityEngine;

namespace ServerSider
{
    public class QuitToLobbyButton : TweakBase
    {
        public override bool allowed => Plugin.Enabled && quitToLobbyButton.Value;
        private readonly ConfigEntry<bool> quitToLobbyButton;

        private static bool quitToLobbyClicked = false;

        internal QuitToLobbyButton(ConfigFile config)
        {
            quitToLobbyButton = config.Bind<bool>("Tweaks", nameof(quitToLobbyButton), true,
                "TODO");
        }

        protected override void Hook()
        {
            On.RoR2.UI.PauseScreenController.Awake += PauseScreenController_Awake;
            IL.RoR2.UI.PauseScreenController.Update += PauseScreenController_Update;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            On.RoR2.UI.PauseScreenController.Awake -= PauseScreenController_Awake;
            IL.RoR2.UI.PauseScreenController.Update -= PauseScreenController_Update;

            Plugin.Logger.LogDebug($"{nameof(QuitToLobbyButton)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===============================

        private static void PauseScreenController_Awake(On.RoR2.UI.PauseScreenController.orig_Awake orig, PauseScreenController self)
        {
            orig(self);
            PauseScreenController_Awake(self);
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
            btn.onClick.m_PersistentCalls.GetListener(0).arguments.stringArgument = "quit_confirmed_command \"run_end\"";
            // quitToLobbyClicked = true; // need to set thsi when the action is confirmed...
        }

        private static void PauseScreenController_Update(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            System.Func<Instruction, bool>[] match = {
                // if (!NetworkManager.singleton.isNetworkActive)
                x => x.MatchLdsfld<UnityEngine.Networking.NetworkManager>(nameof(UnityEngine.Networking.NetworkManager.singleton)),
                x => x.MatchLdfld<UnityEngine.Networking.NetworkManager>(nameof(UnityEngine.Networking.NetworkManager.isNetworkActive)),
                x => x.MatchBrtrue(out _),
                // Object.Destroy(base.gameObject)
                x => x.MatchLdarg(0),
                x => x.MatchCallOrCallvirt<UnityEngine.GameObject>("get_" + nameof(UnityEngine.GameObject.gameObject)),
                x => x.MatchCallOrCallvirt<UnityEngine.Object>(nameof(UnityEngine.Object.Destroy))
            };

            if (c.TryGotoNext(MoveType.After, match)) {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<System.Action<GameObject>>((pauseScreenControllerObject) => {
                    if (UnityEngine.Networking.NetworkManager.singleton.isNetworkActive) {
                        Object.Destroy(pauseScreenControllerObject);
                    }
                });
            }
            else Plugin.Logger.LogError($"{nameof(QuitToLobbyButton)}> Cannot add hook to destroy pause menu: failed to match IL instructions.");

            Plugin.Logger.LogDebug(il.ToString());
        }

#if DEBUG
        [HarmonyLib.HarmonyPostfix, HarmonyLib.HarmonyPatch(typeof(PauseScreenController), nameof(PauseScreenController.OnDisable))]
        private static void PauseScreenController_OnDisable()
        {
            Plugin.Logger.LogInfo(new System.Diagnostics.StackTrace());
        }
#endif
    }
}
