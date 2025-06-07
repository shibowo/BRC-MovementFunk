using HarmonyLib;
using MovementFunk.Mechanics;
using Reptile;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class AirDashAbilityPatch
    {
        private static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

        private static bool showBoost = false;

        [HarmonyPatch(typeof(AirDashAbility), nameof(AirDashAbility.OnStartAbility))]
        public static class AirDashAbility_OnStartAbility_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool modifiedNum = false;
                bool modifiedSetVelocity = false;

                for (int i = 0; i < codes.Count - 3; i++)
                {
                    if (!modifiedNum && codes[i].Calls(AccessTools.Method(typeof(Vector3), nameof(Vector3.Dot))) &&
                        codes[i + 1].opcode == OpCodes.Stloc_3)
                    {
                        codes.InsertRange(i + 2, new[]
                        {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldloc_3),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AirDashAbility_OnStartAbility_Transpiler), nameof(ModifyNumValue))),
                    new CodeInstruction(OpCodes.Stloc_3)
                });
                        modifiedNum = true;
                        i += 4;
                    }

                    if (!modifiedSetVelocity && codes[i].opcode == OpCodes.Ldfld &&
                        codes[i].operand is FieldInfo field && field.Name == "airDashSpeed" &&
                        codes[i + 1].opcode == OpCodes.Mul &&
                        codes[i + 2].opcode == OpCodes.Newobj &&
                        codes[i + 2].operand is ConstructorInfo ctor && ctor.DeclaringType == typeof(Vector3) &&
                        codes[i + 3].opcode == OpCodes.Callvirt &&
                        codes[i + 3].operand is MethodInfo method && method.Name == "SetVelocity" &&
                        method.DeclaringType == typeof(Reptile.Player))
                    {
                        codes.Insert(i + 3, new CodeInstruction(OpCodes.Ldarg_0));
                        codes[i + 4] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AirDashAbility_OnStartAbility_Transpiler), nameof(CustomSetVelocity)));
                        codes.Insert(i + 5, new CodeInstruction(OpCodes.Ret));
                        modifiedSetVelocity = true;
                        break;
                    }
                }

                return codes;
            }

            private static float ModifyNumValue(AirDashAbility airDashAbility, float originalNum)
            {
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                if (airDashAbility.p.isAI && !MovementFunkPlugin.ConfigSettings.Misc.airDashChangeEnabled.Value || ConfigSettings.Misc.DisablePatch.Value) { return originalNum; }
                return MFMath.Remap(originalNum, -1f, 1f, ConfigSettings.Misc.airDashStrength.Value, 1f);
            }

            private static void CustomSetVelocity(Player player, Vector3 velocity, AirDashAbility airDashAbility)
            {
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                Vector3 vector = airDashAbility.p.moveInput.sqrMagnitude == 0f
                    ? (airDashAbility.dirIfNoSteer ?? airDashAbility.p.dir)
                    : airDashAbility.p.moveInput;
                vector = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;

                bool isDoubleJump = !airDashAbility.p.isAI && ConfigSettings.Misc.airDashDoubleJumpEnabled.Value && airDashAbility.p.moveInput.sqrMagnitude == 0f && !ConfigSettings.Misc.DisablePatch.Value;

                if (isDoubleJump)
                {
                    ApplyDoubleJump(airDashAbility, vector);
                }
                else
                {
                    ApplyNormalAirDash(airDashAbility, vector);
                }

                airDashAbility.haveAirDash = false;
                airDashAbility.p.wallrunAbility.cooldownTimer = 0f;
                airDashAbility.p.wallrunAbility.wallrunLine = null;
                if (ConfigSettings.FastFall.ResetOnDash.Value)
                {
                    Fastfall.canFastFall = true;
                }
            }

            private static void ApplyDoubleJump(AirDashAbility airDashAbility, Vector3 vector)
            {
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                float newYVelocity = CalculateNewYVelocity(airDashAbility);
                airDashAbility.p.SetVelocity(new Vector3(airDashAbility.p.GetVelocity().x, newYVelocity, airDashAbility.p.GetVelocity().z));

                airDashAbility.targetSpeed = airDashAbility.p.GetForwardSpeed();
                string animationToPlay = airDashAbility.p.moveStyle == MoveStyle.INLINE ? "airTrick0" : ConfigSettings.Misc.airDashDoubleJumpAnim.Value;
                airDashAbility.p.PlayAnim(MFAnimation.GetAnimationByName(animationToPlay), true, true, -1f);
                airDashAbility.p.audioManager.PlayVoice(ref airDashAbility.p.currentVoicePriority, airDashAbility.p.character, AudioClipID.VoiceJump, airDashAbility.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
                airDashAbility.p.DoHighJumpEffects(airDashAbility.p.tf.up);
                showBoost = false;
            }

            private static void ApplyNormalAirDash(AirDashAbility airDashAbility, Vector3 vector)
            {
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                Vector3 newVelocity = new Vector3(
                    vector.x * airDashAbility.airDashSpeed,
                    Mathf.Max(airDashAbility.airDashInitialUpSpeed, airDashAbility.p.GetVelocity().y),
                    vector.z * airDashAbility.airDashSpeed
                );
                airDashAbility.p.SetVelocity(newVelocity);
                airDashAbility.targetSpeed = airDashAbility.airDashSpeed;
                airDashAbility.p.PlayAnim(airDashAbility.airDashHash, true, false, -1f);
                Core.Instance.AudioManager.PlaySfxGameplay(SfxCollectionID.GenericMovementSfx, AudioClipID.airdash, airDashAbility.p.playerOneShotAudioSource, 0f);
                showBoost = !airDashAbility.p.isAI && ConfigSettings.Misc.airDashDoubleJumpEnabled.Value && !ConfigSettings.Misc.DisablePatch.Value;
            }

            private static float CalculateNewYVelocity(AirDashAbility airDashAbility)
            {
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                float currentYVelocity = airDashAbility.p.GetVelocity().y;
                float jumpAmount = ConfigSettings.Misc.airDashDoubleJumpAmount.Value;

                switch (ConfigSettings.Misc.airDashDoubleJumpType.Value)
                {
                    case MovementConfig.DoubleJumpType.Additive:
                        return Mathf.Max(currentYVelocity + jumpAmount, jumpAmount);

                    case MovementConfig.DoubleJumpType.Replace:
                        return jumpAmount;

                    case MovementConfig.DoubleJumpType.Capped:
                        return MFMath.LosslessClamp(Mathf.Max(currentYVelocity, 0f), jumpAmount, jumpAmount);

                    default:
                        return currentYVelocity;
                }
            }
        }

        [HarmonyPatch(typeof(AirDashAbility), nameof(AirDashAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void AirDashAbility_FixedUpdateAbility_Postfix(AirDashAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (!showBoost)
            {
                __instance.p.SetBoostpackAndFrictionEffects(BoostpackEffectMode.OFF, FrictionEffectMode.OFF);
            }
        }
    }
}
