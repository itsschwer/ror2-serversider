using BepInEx;
using RoR2;

namespace ServerSider
{
    [BepInPlugin(GUID, Name, Version)]

    public sealed class Plugin : BaseUnityPlugin
    {
        public const string GUID = Author + "." + Name;
        public const string Author = "itsschwer";
        public const string Name = "ServerSider";
        public const string Version = "0.0.0";

        public static new Config Config { get; private set; }

        private static Plugin Instance;
        /// <summary>
        /// Wrapper for <see cref="UnityEngine.Behaviour.enabled"/>.
        /// </summary>
        public static bool Enabled => Instance.enabled;

        private event System.Action OnManageHooks;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void Awake()
        {
            Log.Init(Logger);
            Config = new Config(base.Config);

            Instance = this;
            // Use run start/end events to run check for if plugin should be active
            Run.onRunStartGlobal += SetPluginActiveState;
            Run.onRunDestroyGlobal += SetPluginActiveState;
            SetPluginActiveState();

            SetupHooks();

            Log.Message("~awake.");
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
            Log.Message($"~{(value ? "active" : "inactive")}.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnEnable()
        {
            OnManageHooks?.Invoke();
            Log.Message("~enabled.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Message")]
        private void OnDisable()
        {
            OnManageHooks?.Invoke();
            Log.Message("~disabled.");
        }


        private void SetupHooks()
        {
            OnManageHooks += RescueShipLoopPortal.ManageHook;
            OnManageHooks += VoidFieldFogTweak.ManageHook;
        }
    }
}
