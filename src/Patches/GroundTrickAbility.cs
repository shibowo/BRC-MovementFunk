using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MovementFunk.Patches
{
    internal static class GroundTrickAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void GroundTrickAbility_OnStartAbility_Postfix(GroundTrickAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            __instance.decc = MovementSettings.Misc.groundTrickDecc.Value;

            bool isOnFoot = __instance.p.moveStyle == MoveStyle.ON_FOOT;
            bool shouldAllowJumpOnFoot = MovementSettings.Misc.JumpGroundTrickFoot.Value;
            bool shouldAllowJumpOnMovestyle = MovementSettings.Misc.JumpGroundTrickMove.Value;

            __instance.allowNormalJump = (isOnFoot && shouldAllowJumpOnFoot) ||
                                         (!isOnFoot && shouldAllowJumpOnMovestyle);
        }

        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void GroundTrickAbility_FixedUpdateAbility_Postfix(GroundTrickAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) return;

            if (ShouldActivateSpecialAirAbility(__instance))
            {
                __instance.p.ActivateAbility(__instance.p.specialAirAbility);
            }

            if (__instance.p.IsGrounded()) return;

            SetButtslapType(__instance.p.preAbility);
        }

        private static bool ShouldActivateSpecialAirAbility(GroundTrickAbility instance)
        {
            return instance.p.jumpButtonNew &&
                   instance.allowNormalJump &&
                   !instance.p.specialAirAbility.locked &&
                   !instance.p.onLauncher &&
                   (instance.p.moveStyle == MoveStyle.ON_FOOT || (MovementFunkPlugin.MovementSettings.SuperTrickJump.MSSuperTrick.Value && MSSuperTrickCheck(instance))) &&
                   instance.p.IsGrounded() &&
                   MovementFunkPlugin.MovementSettings.SuperTrickJump.EarlySuperTrick.Value;
        }

        private static bool MSSuperTrickCheck(GroundTrickAbility instance)
        {
            if (instance.p.abilityTimer < instance.duration - instance.reTrickMargin - (instance.hitEnemy ? 0.06f : 0f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HarmonyPatch(typeof(GroundTrickAbility), nameof(GroundTrickAbility.FixedUpdateAbility))]
        public static class GroundTrickAbility_FixedUpdateAbility_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].LoadsField(AccessTools.Field(typeof(Ability), "p")) &&
                        codes[i + 2].Calls(AccessTools.Method(typeof(Player), nameof(Player.Jump))) &&
                        codes[i + 3].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 4].LoadsField(AccessTools.Field(typeof(Ability), "p")) &&
                        codes[i + 5].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 6].LoadsField(AccessTools.Field(typeof(Ability), "p")) &&
                        codes[i + 7].LoadsField(AccessTools.Field(typeof(Player), "airTrickAbility")) &&
                        codes[i + 8].Calls(AccessTools.Method(typeof(Player), nameof(Player.ActivateAbility))))
                    {
                        codes[i + 2] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GroundTrickAbility_FixedUpdateAbility_Transpiler), nameof(CustomJump)));

                        codes[i + 8] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(GroundTrickAbility_FixedUpdateAbility_Transpiler), nameof(CustomActivateAbility)));

                        break;
                    }
                }

                return codes;
            }

            private static void CustomJump(Player player)
            {
                if (player.isAI || !MovementFunkPlugin.MovementSettings.SuperTrickJump.MSSuperTrick.Value || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value)
                {
                    player.Jump();
                }
                else
                {
                    player.ActivateAbility(player.specialAirAbility);
                }
            }

            private static void CustomActivateAbility(Player player, Ability ability)
            {
                if (player.isAI || !MovementFunkPlugin.MovementSettings.SuperTrickJump.MSSuperTrick.Value || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value)
                {
                    player.ActivateAbility(ability);
                }
            }
        }

        private static void SetButtslapType(Ability preAbility)
        {
            switch (preAbility)
            {
                case MovementFunk.NewAbility.SurfAbility:
                    MFVariables.buttslapType = "Surf";
                    break;

                case Reptile.HandplantAbility:
                    MFVariables.buttslapType = "Pole";
                    break;

                default:
                    MFVariables.buttslapType = "Ground";
                    break;
            }
        }
    }
}
