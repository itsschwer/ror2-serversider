using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Reflection;

namespace ServerSider
{
    public class ChanceDollMessage : TweakBase
    {
        public override bool allowed => Plugin.Enabled && chanceDollMessage.Value;
        private readonly ConfigEntry<bool> chanceDollMessage;

        private static readonly FieldInfo chanceDollWin = typeof(ShrineChanceBehavior).GetField(nameof(chanceDollWin), BindingFlags.Instance | BindingFlags.NonPublic);

        internal ChanceDollMessage(ConfigFile config)
        {
            chanceDollMessage = config.Bind<bool>("Tweaks", nameof(chanceDollMessage), true,
                "Reword the Shrine of Chance success message to indicate if a Chance Doll affected the reward.");
        }

        protected override void Hook()
        {
            if (chanceDollWin == null) {
                Plugin.Logger.LogWarning($"{nameof(ChanceDollMessage)}> Cannot hook: no {nameof(chanceDollWin)} field.");
                return;
            }

            IL.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;

            Plugin.Logger.LogDebug($"{nameof(ChanceDollMessage)}> Hooked by {GetExecutingMethod()}");
        }

        protected override void Unhook()
        {
            IL.RoR2.ShrineChanceBehavior.AddShrineStack -= ShrineChanceBehavior_AddShrineStack;

            Plugin.Logger.LogDebug($"{nameof(ChanceDollMessage)}> Unhooked by {GetExecutingMethod()}");
        }

        // Functionality ===============================

        private static void ShrineChanceBehavior_AddShrineStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            Func<Instruction, bool>[] match = {
                x => x.MatchNewobj<Chat.SubjectFormatChatMessage>(),
                x => x.MatchDup(),
                x => x.MatchLdloc(1),
                x => x.MatchCallOrCallvirt<SubjectChatMessage>("set_" + nameof(SubjectChatMessage.subjectAsCharacterBody)),
                x => x.MatchDup(),
                x => x.MatchLdloc(4),
                x => x.MatchStfld<SubjectChatMessage>(nameof(SubjectChatMessage.baseToken)),
                x => x.MatchCallOrCallvirt(typeof(Chat), nameof(Chat.SendBroadcastChat))
            };

            bool matched = c.TryGotoNext(match);

            if (matched) {
                ILLabel originalBroadcast = c.MarkLabel();
                // if (chanceDollWin)
                c.Emit(OpCodes.Ldarg_0);
                c.Emit<ShrineChanceBehavior>(OpCodes.Ldfld, nameof(chanceDollWin));
                c.Emit(OpCodes.Brfalse_S, originalBroadcast);
                // send alternate message
                c.Emit(OpCodes.Ldloc_1);
                c.Emit(OpCodes.Ldloc, 4);
                c.EmitDelegate<Action<CharacterBody, string>>((activator, baseToken) => {
                    // Use SimpleChatMessage -- can't use SubjectFormatChatMessage without adding custom language tokens (including on clients)
                    string baseMessage = Language.GetStringFormatted(baseToken, [activator.GetUserName()]);
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage {
                        baseToken = baseMessage.Replace("!", (Language.currentLanguage == Language.english) ? " greatly!" : "!+") // untested on languages other than English
                    });
                });

                ILLabel end = c.MarkLabel();
                c.Emit(OpCodes.Br_S, end);

                c.MarkLabel(originalBroadcast);
                c.GotoNext(MoveType.After, match);
                c.MarkLabel(end);
#if DEBUG
                Plugin.Logger.LogDebug(il.ToString());
#endif
            }
            else Plugin.Logger.LogError($"{nameof(ChanceDollMessage)}> Cannot hook: failed to match IL instructions.");
        }
    }
}
