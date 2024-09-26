﻿using BepInEx;
using RoR2;

namespace ServerSider
{
    [BepInPlugin(GUID, Name, Version)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "ServerSider";
        public const string Version = "1.1.0";

        internal static new BepInEx.Logging.ManualLogSource Logger { get; private set; }

        public static TweakManager Tweaks { get; private set; }

        private static Plugin Instance;
        internal static bool Enabled => Instance.enabled;


        private void Awake()
        {
            // Use Plugin.GUID instead of Plugin.Name as source name
            BepInEx.Logging.Logger.Sources.Remove(base.Logger);
            Logger = BepInEx.Logging.Logger.CreateLogSource(Plugin.GUID);

            Tweaks = new TweakManager(Config);

            Instance = this;
            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();

            Logger.LogMessage("~awake.");
        }

        private void OnEnable()
        {
            Logger.LogDebug($"Reloading {Config.ConfigFilePath.Substring(Config.ConfigFilePath.LastIndexOf(System.IO.Path.DirectorySeparatorChar) + 1)}");
            Config.Reload();

            Tweaks.Refresh();
            Logger.LogMessage("~enabled.");
        }

        private void OnDisable()
        {
            Tweaks.Refresh();
            Logger.LogMessage("~disabled.");
        }

        /// <summary>
        /// Wrapper for <see cref="SetActive"/>, passing in <see cref="UnityEngine.Networking.NetworkServer.active"/>,
        /// which appears to be used for determining if client is host.
        /// </summary>
        private void SetPluginActiveState(Run _ = null) => SetActive(UnityEngine.Networking.NetworkServer.active);

        /// <summary>
        /// All plugins are attached to the
        /// <see href="https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Bootstrap/Chainloader.cs#L88">same</see>
        /// <see cref="UnityEngine.GameObject"/>, so manually manage components instead of calling <see cref="UnityEngine.GameObject.SetActive"/>.
        /// </summary>
        private void SetActive(bool value)
        {
            this.enabled = value;
            Logger.LogMessage($"~{(value ? "active" : "inactive")}.");
        }


#if DEBUG
        private void Update()
        {
            CharacterBody body = LocalUserManager.GetFirstLocalUser().currentNetworkUser.GetCurrentBody();
            if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Insert)) TestRescueShipLoopPortal(body);
        }

        private static void TestRescueShipLoopPortal(CharacterBody target)
        {
            InputBankTest input = target?.inputBank;
            if (input == null) return;

            RescueShipLoopPortal.InstantiatePortal(target.footPosition, UnityEngine.Quaternion.LookRotation(-input.aimDirection));
        }
#endif
    }
}
