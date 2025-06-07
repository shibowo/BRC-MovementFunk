using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class WallrunLineAbilityPatch
    {
        public static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

        private static float defaultMoveSpeed;

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Init))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Init_Postfix(WallrunLineAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            __instance.minDurationBeforeJump = ConfigSettings.WallGeneral.minDurJump.Value;
            __instance.wallrunDecc = ConfigSettings.WallGeneral.decc.Value;
            defaultMoveSpeed = __instance.wallRunMoveSpeed;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_OnStartAbility_Prefix(WallrunLineAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.RunOff))]
        [HarmonyPrefix]
        private static bool WallrunLineAbility_RunOff_Prefix(WallrunLineAbility __instance, Vector3 direction)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return true; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            Vector3 vector;
            if (__instance.p.abilityTimer <= ConfigSettings.WallFrameboost.Grace.Value)
            {
                if (ConfigSettings.WallFrameboost.Enabled.Value && ConfigSettings.WallFrameboost.RunoffEnabled.Value)
                {
                    float newSpeed = MFMath.LosslessClamp(Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude), ConfigSettings.WallFrameboost.Amount.Value, ConfigSettings.WallFrameboost.Cap.Value);
                    vector = direction * (newSpeed) + __instance.wallrunFaceNormal * 1f;
                    __instance.p.DoTrick(Player.TrickType.WALLRUN, "Frameboost", 0);
                }
                else
                {
                    vector = direction * (Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude)) + __instance.wallrunFaceNormal * 1f;
                }
                __instance.lastSpeed = Mathf.Max(MFVariables.savedGoon, __instance.lastSpeed);

                MFVariables.savedGoon = __instance.lastSpeed;
            }
            else
            {
                vector = direction * (Mathf.Max(__instance.lastSpeed, __instance.customVelocity.magnitude, 13f)) + __instance.wallrunFaceNormal * 1f;
                MFVariables.savedGoon = __instance.lastSpeed;
            }
            __instance.p.SetVelocity(vector);
            __instance.p.SetRotHard(Quaternion.LookRotation(vector.normalized));
            __instance.p.FlattenRotationHard();
            if (__instance.p.boosting)
            {
                __instance.p.ActivateAbility(__instance.p.boostAbility);
                __instance.p.boostAbility.StartFromRunOffGrindOrWallrun();
                return false;
            }
            __instance.p.StopCurrentAbility();
            __instance.p.PlayAnim(__instance.fallHash, false, false, -1f);
            return false;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static bool WallrunLineAbility_FixedUpdateAbility_Prefix(WallrunLineAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return true; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings; __instance.UpdateBoostpack();
            __instance.scoreTimer += Core.dt;
            if (__instance.p.abilityTimer <= 0.025f && !__instance.p.isJumping)
            {
                float newSpeed = MFMath.LosslessClamp(MFMovementMetrics.AverageForwardSpeed(), MFMovementMetrics.AverageTotalSpeed() - MFMovementMetrics.AverageForwardSpeed(), ConfigSettings.WallGeneral.wallTotalSpeedCap.Value);
                __instance.speed = Mathf.Max(newSpeed, __instance.wallRunMoveSpeed);
            }
            if (__instance.scoreTimer > 0.7f)
            {
                __instance.scoreTimer = 0f;
                __instance.p.DoTrick(Player.TrickType.WALLRUN, __instance.trickName, 0);
            }
            if (__instance.speed > __instance.wallRunMoveSpeed)
            {
                __instance.speed = Mathf.Max(__instance.wallRunMoveSpeed, __instance.speed - __instance.wallrunDecc * Core.dt);
            }
            if (__instance.p.boosting)
            {
                __instance.speed = MFMath.LosslessClamp(__instance.speed, ConfigSettings.BoostGeneral.WallAmount.Value * Core.dt, ConfigSettings.BoostGeneral.WallCap.Value);
            }
            __instance.journey += __instance.speed / __instance.nodeToNodeLength * Core.dt;
            __instance.wallrunPos = Vector3.LerpUnclamped(__instance.prevNode.position, __instance.nextNode.position, __instance.journey);
            __instance.dirToNextNode = (__instance.nextNode.position - __instance.prevNode.position).normalized;
            __instance.wallrunFaceNormal = __instance.wallrunLine.transform.forward;
            float num = -__instance.p.motor.GetCapsule().height * 1f;
            if (__instance.wallrunHeight > num)
            {
                if (__instance.wallRunUpDownSpeed > 0f)
                {
                    __instance.wallRunUpDownSpeed = Mathf.Max(__instance.wallRunUpDownSpeed - __instance.wallRunUpDownDecc * Core.dt, 0f);
                }
                else if (__instance.wallRunUpDownSpeed < 0f)
                {
                    __instance.wallRunUpDownSpeed = Mathf.Min(__instance.wallRunUpDownSpeed + __instance.wallRunUpDownDecc * Core.dt, 0f);
                }
            }
            else
            {
                __instance.wallRunUpDownSpeed = __instance.wallRunUpDownMaxSpeed;
            }
            __instance.wallrunHeight += __instance.wallRunUpDownSpeed * Core.dt;
            Vector3 vector = __instance.wallrunPos + __instance.wallrunHeight * Vector3.up + __instance.wallrunFaceNormal * __instance.p.motor.GetCapsule().radius;
            __instance.customVelocity = (vector - __instance.p.tf.position) / Core.dt;
            __instance.lastSpeed = __instance.customVelocity.magnitude;

            __instance.p.lastElevationForSlideBoost = vector.y;
            Vector3 normalized = Vector3.RotateTowards(__instance.p.dir, __instance.dirToNextNode, __instance.rotSpeed * Core.dt, 1000f).normalized;
            if (__instance.p.smoothRotation)
            {
                __instance.p.SetRotation(normalized);
            }
            else
            {
                __instance.p.SetRotHard(normalized);
            }
            if (__instance.p.jumpButtonNew && (__instance.p.abilityTimer > __instance.minDurationBeforeJump || __instance.p.isAI))
            {
                __instance.Jump();
            }
            else if (__instance.journey > 1f)
            {
                __instance.AtEndOfWallrunLine();
            }
            __instance.p.SetVisualRotLocal0();
            MFVariables.savedLastSpeed = __instance.lastSpeed;
            if (__instance.p.abilityTimer > MovementFunkPlugin.ConfigSettings.WallFrameboost.Grace.Value && MovementFunkPlugin.ConfigSettings.WallFrameboost.Enabled.Value)
            {
                MFVariables.savedGoon = __instance.lastSpeed;
            }
            return false;
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.Jump))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_Jump_Postfix(WallrunLineAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (__instance.p.abilityTimer <= ConfigSettings.WallBoostplant.Grace.Value && ConfigSettings.WallBoostplant.Enabled.Value)
            {
                List<string> buttons = MFMisc.StringToList(ConfigSettings.WallBoostplant.Buttons.Value);
                bool buttonsActive = MFInputBuffer.WasPressedRecentlyOrIsHeld(buttons, ConfigSettings.WallBoostplant.Buffer.Value);

                if (buttonsActive)
                {
                    float averageSpeed = MFMovementMetrics.AverageTotalSpeed();
                    float currentSpeed = __instance.p.GetTotalSpeed();
                    float baseSpeed = Mathf.Max(averageSpeed, currentSpeed);

                    float jumpHeight = baseSpeed * ConfigSettings.WallBoostplant.Strength.Value;
                    float forwardSpeed = Mathf.Max(baseSpeed - (jumpHeight * ConfigSettings.WallBoostplant.SpeedStrength.Value), 0f);

                    __instance.p.SetForwardSpeed(forwardSpeed);
                    __instance.p.motor.SetVelocityYOneTime(jumpHeight);

                    __instance.p.DoTrick(Player.TrickType.WALLRUN, "Boostplant", 0);

                    __instance.lastSpeed = Mathf.Max(MFVariables.savedGoon, __instance.lastSpeed);
                    MFVariables.savedGoon = __instance.lastSpeed;
                    return;
                }

                if (__instance.p.abilityTimer <= ConfigSettings.WallFrameboost.Grace.Value && ConfigSettings.WallFrameboost.Enabled.Value)
                {
                    float newSpeed = MFMath.LosslessClamp(Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), __instance.p.GetForwardSpeed()), ConfigSettings.WallFrameboost.Amount.Value, ConfigSettings.WallFrameboost.Cap.Value);
                    __instance.lastSpeed += ConfigSettings.WallFrameboost.Amount.Value;
                    __instance.p.SetForwardSpeed(newSpeed);
                    __instance.p.DoTrick(Player.TrickType.WALLRUN, "Frameboost", 0);
                    __instance.lastSpeed = Mathf.Max(MFVariables.savedGoon, __instance.lastSpeed);
                    MFVariables.savedGoon = __instance.lastSpeed;
                }
            }
        }

        [HarmonyPatch(typeof(WallrunLineAbility), nameof(WallrunLineAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void WallrunLineAbility_OnStopAbility_Postfix(WallrunLineAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            __instance.cooldownTimer = MovementFunkPlugin.ConfigSettings.WallGeneral.wallCD.Value;
        }
    }
}
