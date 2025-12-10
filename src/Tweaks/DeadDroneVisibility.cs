using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace ServerSider
{
    public class DeadDroneVisibility : TweakBase
    {
        public override bool allowed => Plugin.Enabled && deadDroneVisibility.Value;
        private readonly ConfigEntry<bool> deadDroneVisibility;

        internal DeadDroneVisibility(ConfigFile config)
        {
            deadDroneVisibility = config.Bind<bool>("Tweaks", nameof(deadDroneVisibility), true,
                "TODO");
        }

        protected override void Hook()
        {
            IL.EntityStates.Drone.DeathState.OnImpactServer += Drone_DeathState_OnImpactServer;

            Plugin.Logger.LogDebug($"{nameof(DeadDroneVisibility)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            IL.EntityStates.Drone.DeathState.OnImpactServer -= Drone_DeathState_OnImpactServer;

            Plugin.Logger.LogDebug($"{nameof(DeadDroneVisibility)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===================================

        private static void Drone_DeathState_OnImpactServer(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int loc = -1;

            Func<Instruction, bool>[] match = {
                // GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, placementRule, new Xoroshiro128Plus(0uL)));
                x => x.MatchCallOrCallvirt<DirectorCore>($"get_{nameof(DirectorCore.instance)}"),
                x => x.MatchLdloc(out int _),
                x => x.MatchLdloc(out int _),
                x => x.MatchLdcI4(out int _),
                x => x.MatchConvI8(),
                x => x.MatchNewobj(out _),
                x => x.MatchNewobj(out _),
                x => x.MatchCallOrCallvirt<DirectorCore>(nameof(DirectorCore.TrySpawnObject)),
                x => x.MatchStloc(out loc),
                // if ((bool)gameObject)
                x => x.MatchLdloc(loc),
                x => x.MatchCallOrCallvirt<UnityEngine.Object>("op_Implicit"),
                x => x.MatchBrfalse(out ILLabel _)
            };

            if (c.TryGotoNext(MoveType.After, match)) {
                // TODO
                // need networkspawn an existing usable beam prefab?
                // or maybe networkspawn a shield wall power pedestal pyramid and steal the beam vfx?
                // (is reparenting game objects and destroying leftovers feasible (synced from server to clients)?)

                // "RoR2/DLC3/PowerOrbPedestal/PowerPedestal_Pyramid.prefab"

                c.Emit(OpCodes.Ldloc, loc);
                c.EmitDelegate<Action<UnityEngine.GameObject>>((spawned) => {
                    Plugin.Logger.LogWarning(spawned.name);
                    var go = new UnityEngine.GameObject("beam", typeof(UnityEngine.LineRenderer));
                    go.transform.parent = spawned.transform;
                    var lr = go.GetComponent<UnityEngine.LineRenderer>();
                    lr.SetPositions([spawned.transform.position, spawned.transform.position + UnityEngine.Vector3.up * 100]);
                    lr.startWidth = 0.2f;
                    lr.endWidth = 0f;
                    lr.startColor = UnityEngine.Color.red;
                    lr.endColor = UnityEngine.Color.yellow;
                    lr.material = new UnityEngine.Material(LegacyShaderAPI.Find("Hopoo Games/FX/Vertex Colors Only"));
                    UnityEngine.Networking.NetworkServer.Spawn(go);
                    Plugin.Logger.LogWarning($"verify: {UnityEngine.Networking.NetworkServer.VerifyCanSpawn(go)}");
                });
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(DeadDroneVisibility)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
