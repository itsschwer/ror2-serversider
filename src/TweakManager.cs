using System.Collections.Generic;

namespace ServerSider
{
    public class TweakManager
    {
        /// <summary>
        /// The list of tweaks that this mod manages the hooks for.
        /// </summary>
        /// <remarks>
        /// Remove a tweak from this list if you plan on taking over its hook management
        /// (e.g. turning a tweak into an Artifact).
        /// </remarks>
        public readonly List<TweakBase> managedTweaks;

        public readonly RescueShipLoopPortal RescueShipLoopPortal;
        public readonly PressurePlateTweak PressurePlateTweak;
        public readonly QuitToLobbyButton QuitToLobbyButton;
        public readonly VoidPickupConfirmAll VoidPickupConfirmAll;
        public readonly VoidFieldFogTweak VoidFieldFogTweak;
        public readonly SendItemCostInChat SendItemCostInChat;
        public readonly TeleportOutOfBoundsPickups TeleportOutOfBoundsPickups;
        public readonly PreventEarlyHalcyonShrineActivation PreventEarlyHalcyonShrineActivation;

        internal TweakManager(BepInEx.Configuration.ConfigFile config)
        {
            managedTweaks = new List<TweakBase>();

            RescueShipLoopPortal = new RescueShipLoopPortal(config);
            managedTweaks.Add(RescueShipLoopPortal);

            PressurePlateTweak = new PressurePlateTweak(config);
            managedTweaks.Add(PressurePlateTweak);

            QuitToLobbyButton = new QuitToLobbyButton(config);
            managedTweaks.Add(QuitToLobbyButton);

            VoidPickupConfirmAll = new VoidPickupConfirmAll(config);
            managedTweaks.Add(VoidPickupConfirmAll);

            VoidFieldFogTweak = new VoidFieldFogTweak(config);
            managedTweaks.Add(VoidFieldFogTweak);

            SendItemCostInChat = new SendItemCostInChat(config);
            managedTweaks.Add(SendItemCostInChat);

            TeleportOutOfBoundsPickups = new TeleportOutOfBoundsPickups(config);
            managedTweaks.Add(TeleportOutOfBoundsPickups);

            PreventEarlyHalcyonShrineActivation = new PreventEarlyHalcyonShrineActivation(config);
            managedTweaks.Add(PreventEarlyHalcyonShrineActivation);
        }

        internal void Refresh()
        {
            for (int i = 0; i < managedTweaks.Count; i++) {
                managedTweaks[i].Disable();
                if (managedTweaks[i].allowed) managedTweaks[i].Enable();
            }
        }
    }
}
