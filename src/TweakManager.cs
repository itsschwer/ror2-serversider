﻿using System.Collections.Generic;

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
        public readonly VoidFieldFogTweak VoidFieldFogTweak;
        public readonly QuitToLobbyButton QuitToLobbyButton;
        public readonly SendItemCostInChat SendItemCostInChat;
        public readonly ChanceDollMessage ChanceDollMessage;
        public readonly TeleportOutOfBoundsPickups TeleportOutOfBoundsPickups;

        internal TweakManager(BepInEx.Configuration.ConfigFile config)
        {
            managedTweaks = new List<TweakBase>();

            RescueShipLoopPortal = new RescueShipLoopPortal(config);
            managedTweaks.Add(RescueShipLoopPortal);

            PressurePlateTweak = new PressurePlateTweak(config);
            managedTweaks.Add(PressurePlateTweak);

            VoidFieldFogTweak = new VoidFieldFogTweak(config);
            managedTweaks.Add(VoidFieldFogTweak);

            QuitToLobbyButton = new QuitToLobbyButton(config);
            managedTweaks.Add(QuitToLobbyButton);

            SendItemCostInChat = new SendItemCostInChat(config);
            managedTweaks.Add(SendItemCostInChat);

            ChanceDollMessage = new ChanceDollMessage(config);
            managedTweaks.Add(ChanceDollMessage);

            TeleportOutOfBoundsPickups = new TeleportOutOfBoundsPickups(config);
            managedTweaks.Add(TeleportOutOfBoundsPickups);
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
