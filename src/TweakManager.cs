using System.Collections.Generic;

namespace ServerSider
{
    public class TweakManager
    {
        public List<TweakBase> managedTweaks;

        public readonly RescueShipLoopPortal RescueShipLoopPortal;
        public readonly VoidFieldFogTweak VoidFieldFogTweak;
        public readonly ChanceDollMessage ChanceDollMessage;

        internal TweakManager(BepInEx.Configuration.ConfigFile config)
        {
            managedTweaks = new List<TweakBase>();

            RescueShipLoopPortal = new RescueShipLoopPortal(config);
            managedTweaks.Add(RescueShipLoopPortal);

            VoidFieldFogTweak = new VoidFieldFogTweak(config);
            managedTweaks.Add(VoidFieldFogTweak);

            ChanceDollMessage = new ChanceDollMessage(config);
            managedTweaks.Add(ChanceDollMessage);
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
