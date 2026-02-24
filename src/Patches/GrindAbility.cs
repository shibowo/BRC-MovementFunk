using HarmonyLib;
using MovementFunk.Mechanics;
using Reptile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class GrindAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;
        public static float lastCornerTime;

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.Init))]
        [HarmonyPrefix]
        private static void GrindAbility_Init_Prefix(GrindAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            __instance.grindDeccAboveNormal = MovementSettings.RailGeneral.Decc.Value;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.GetPlayerDirForLine))]
        [HarmonyPrefix]
        private static bool GrindAbility_GetPlayerDirForLine_Prefix(GrindAbility __instance, GrindLine setGrindLine, ref Vector3 __result)
        {
            if (__instance.p.isAI || !MovementFunkPlugin.MovementSettings.RailGeneral.ChangeDirectionEnabled.Value || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return true; }

            Vector3 playerForward = Vector3.ProjectOnPlane(__instance.p.GetVelocity(), Vector3.up);
            if (playerForward == Vector3.zero)
            {
                playerForward = __instance.p.dir;
            }
            playerForward = playerForward.normalized;

            Vector3 inputDir = __instance.p.moveInput;
            Vector3 vector;

            if (inputDir.magnitude > 0.1f)
            {
                vector = inputDir.normalized;
            }
            else
            {
                vector = playerForward;
            }

            if (setGrindLine.isPole)
            {
                vector = Vector3.up;
            }
            else
            {
                float maxAngleChange = MovementFunkPlugin.MovementSettings.RailGeneral.ChangeDirectionAngle.Value;
                float maxDot = Mathf.Cos(maxAngleChange * Mathf.Deg2Rad);

                float dotProduct = Vector3.Dot(playerForward, vector);
                if (dotProduct < maxDot)
                {
                    vector = (playerForward + (vector - playerForward).normalized * maxDot).normalized;
                }
            }

            __result = vector;
            return false;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void GrindAbility_OnStartAbility_Prefix(GrindAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            __instance.trickTimer = 0f;
            __instance.inRetour = false;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.UpdateSpeed))]
        [HarmonyPrefix]
        private static bool GrindAbility_UpdateSpeed_Prefix(GrindAbility __instance)
        {
            if (__instance.p.isAI) { return true; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            if (__instance.p.abilityTimer <= 0.025f && !__instance.p.isJumping)
            {
                float newSpeed = MFMovementMetrics.AverageForwardSpeed();

                newSpeed = Mathf.Max(newSpeed, __instance.speedTarget);
                __instance.p.normalBoostSpeed = newSpeed;

                __instance.speed = Mathf.Max(newSpeed, __instance.speedTarget);
            }
            if (__instance.speed < __instance.speedTarget)
            {
                __instance.speed = Mathf.Min(__instance.speedTarget, __instance.speed + __instance.grindAcc * Core.dt);
            }
            else if (__instance.speed > __instance.speedTarget)
            {
                __instance.speed = Mathf.Max(__instance.speedTarget, __instance.speed - ((__instance.speed >= __instance.p.stats.grindSpeed) ? __instance.grindDeccAboveNormal : __instance.grindDeccBelowNormal) * Core.dt);
            }
            if (__instance.p.boosting)
            {
                float newSpeed = Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), __instance.speed);
                __instance.speed = MFMath.LosslessClamp(newSpeed, MovementSettings.BoostGeneral.RailAmount.Value * Core.dt, MovementSettings.BoostGeneral.RailCap.Value);
            }
            if (__instance.softCornerBoost)
            {
                __instance.speedTarget = Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), __instance.p.GetForwardSpeed());
                if (__instance.timeSinceLastNode > 1f)
                {
                    __instance.softCornerBoost = false;
                }
            }
            else
            {
                if (__instance.p.AirBraking() && !__instance.p.isAI)
                {
                    __instance.braking = true;
                    __instance.speedTarget = __instance.p.stats.grindSpeed * 0.35f;
                }
                __instance.braking = false;
                __instance.speedTarget = __instance.p.stats.grindSpeed;
            }
            return false;
        }

        private static void ApplyCustomSpeedLogic(GrindAbility instance)
        {
            if (instance.p.abilityTimer <= 0.025f && !instance.p.isJumping)
            {
                float newSpeed = MFMovementMetrics.AverageForwardSpeed();
                newSpeed = Mathf.Max(newSpeed, instance.speedTarget);
                instance.p.normalBoostSpeed = newSpeed;
                instance.speed = Mathf.Max(newSpeed, instance.speedTarget);
            }

            if (instance.speed < instance.speedTarget)
            {
                instance.speed = Mathf.Min(instance.speedTarget, instance.speed + instance.grindAcc * Core.dt);
            }
            else if (instance.speed > instance.speedTarget)
            {
                instance.speed = Mathf.Max(
                    instance.speedTarget,
                    instance.speed - ((instance.speed >= instance.p.stats.grindSpeed) ?
                                      instance.grindDeccAboveNormal :
                                      instance.grindDeccBelowNormal) * Core.dt
                );
            }
        }

        private static void ApplyCustomBoostLogic(GrindAbility instance)
        {
            if (instance.p.boosting)
            {
                float newSpeed = Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), instance.speed);
                var config = MovementFunkPlugin.MovementSettings;
                instance.speed = MFMath.LosslessClamp(
                    newSpeed,
                    config.BoostGeneral.RailAmount.Value * Core.dt,
                    config.BoostGeneral.RailCap.Value
                );
            }
            else if (instance.softCornerBoost)
            {
                instance.speedTarget = Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), instance.p.GetForwardSpeed());
                if (instance.timeSinceLastNode > 1f)
                {
                    instance.softCornerBoost = false;
                }
            }
        }

        private static void ApplyCustomBrakingLogic(GrindAbility instance)
        {
            if (instance.p.AirBraking() && !instance.p.isAI)
            {
                instance.braking = true;
                instance.speedTarget = instance.p.stats.grindSpeed * 0.35f;
            }
            else
            {
                instance.braking = false;
                instance.speedTarget = instance.p.stats.grindSpeed;
            }
        }

        private static void ApplyOriginalSpeedLogic(GrindAbility instance)
        {
            if (instance.speed < instance.speedTarget)
            {
                instance.speed = Mathf.Min(instance.speedTarget, instance.speed + instance.grindAcc * Core.dt);
            }
            else if (instance.speed > instance.speedTarget)
            {
                instance.speed = Mathf.Max(
                    instance.speedTarget,
                    instance.speed - ((instance.speed >= instance.p.stats.grindSpeed) ?
                                      instance.grindDeccAboveNormal :
                                      instance.grindDeccBelowNormal) * Core.dt
                );
            }
        }

        private static void ApplyOriginalBoostLogic(GrindAbility instance)
        {
            if (instance.p.boosting)
            {
                instance.speed = (instance.speedTarget = instance.p.boostSpeed);
            }
            else if (instance.softCornerBoost)
            {
                instance.speedTarget = instance.p.boostSpeed;
                if (instance.timeSinceLastNode > 1f)
                {
                    instance.softCornerBoost = false;
                }
            }
        }

        private static void ApplyOriginalBrakingLogic(GrindAbility instance)
        {
            if (instance.p.AirBraking() && !instance.p.isAI)
            {
                instance.braking = true;
                instance.speedTarget = instance.p.stats.grindSpeed * 0.35f;
            }
            else
            {
                instance.braking = false;
                instance.speedTarget = instance.p.stats.grindSpeed;
            }
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void GrindAbility_FixedUpdateAbility_Postfix(GrindAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }

            float currentPos = __instance.grindLine.GetAbsoluteLinePos(__instance.p.tf.position, __instance.p.dir);
            if (__instance.p.abilityTimer >= 0.1f)
            {
                RailGoon.hasGooned = false;
            }
            if (currentPos <= 1.5f && __instance.p.boosting && __instance.p.abilityTimer <= 0.1f && !RailGoon.hasGooned)
            {
                RailGoon.hasGooned = true;
            }
            if (__instance.p.jumpButtonNew && !__instance.inRetour && (__instance.trickTimer > __instance.reTrickMargin || __instance.p.abilityTimer < 0.1f))
            {
                float num6 = Vector3.Dot(__instance.p.tf.up, Vector3.up);
                __instance.JumpOut(__instance.grindLine.isPole || (num6 > -0.25f && num6 < 0.35f));
            }

            if (!MovementFunkPlugin.MovementSettings.RailGeneral.RailReversalEnabled.Value)
            {
                return;
            }

            List<string> buttons = MFMisc.StringToList(MovementFunkPlugin.MovementSettings.RailGeneral.RailReversalButtons.Value);
            bool buttonsActive = MFInputBuffer.WasPressedRecentlyOrIsHeld(buttons, 0.1f);
            if (buttonsActive)
            {
                RailReverse(__instance);
            }
        }

        public static float lastReverseTime = 0f;

        public static void RailReverse(GrindAbility ability)
        {
            if (Time.time - lastReverseTime < MovementFunkPlugin.MovementSettings.RailGeneral.RailReversalCD.Value)
                return;
            ability.timeSinceLastNode = 0f;
            ability.posOnLine = 0f;
            ability.preGrindDir = Vector3.zero;
            ability.grindTilt = Vector2.zero;
            ability.softCornerBoost = false;
            if (ability.nextNode == null) { return; }
            ability.nextNode = ability.grindLine.GetOtherNode(ability.nextNode);
            Vector3 lineDir = ability.grindLine.GetLineDir(ability.nextNode);
            ability.preGrindDir = lineDir;
            ability.p.SetRotHard(Quaternion.LookRotation(lineDir, ability.normal));
            ability.p.PlayAnim(Animator.StringToHash("grindRetourEnd"), true, true, -1f);

            lastReverseTime = Time.time;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.JumpOut))]
        [HarmonyPrefix]
        private static bool GrindAbility_JumpOut_Prefix(bool flipOut, GrindAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) return true;

            SetupJump(__instance);

            if (flipOut)
            {
                if (MovementSettings.RailGeneral.ModifyFlipout.Value)
                    ModifiedFlipOut(__instance, ref flipOut);
                else
                    OriginalFlipOut(__instance, ref flipOut);
            }
            else
            {
                if (MovementSettings.RailGeneral.ModifyJump.Value)
                    ModifiedJump(__instance);
                else
                    OriginalJump(__instance);
            }

            CleanupJump(__instance);

            return false;
        }

        private static void SetupJump(GrindAbility instance)
        {
            instance.p.AudioManager.PlayVoice(ref instance.p.currentVoicePriority, instance.p.character, AudioClipID.VoiceJump, instance.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
            instance.p.timeSinceLastJump = 0f;
            instance.p.isJumping = true;
            instance.p.jumpConsumed = true;
            instance.p.jumpRequested = false;
            instance.p.jumpedThisFrame = true;
            instance.p.maintainSpeedJump = false;
            instance.p.lastElevationForSlideBoost = float.PositiveInfinity;
            Vector3 up = instance.p.tf.up;
            instance.p.DoJumpEffects(up);
            instance.p.tf.position += Vector3.up * instance.p.motor.GetCapsule().height * Mathf.Clamp(instance.p.tf.up.y, -1f, 0f);
        }

        private static void OriginalFlipOut(GrindAbility instance, ref bool flipOut)
        {
            Vector3 up = instance.p.tf.up;
            Vector3 normalized = Vector3.ProjectOnPlane(up, Vector3.up).normalized;
            instance.p.SetRotHard(Quaternion.LookRotation(normalized));
            float num = instance.p.jumpSpeed * 0.35f;
            float num2 = instance.p.maxMoveSpeed * 0.57f;
            instance.p.SetVelocity(num * Vector3.up + normalized * num2);
            instance.p.ActivateAbility(instance.p.flipOutJumpAbility);
        }

        private static void ModifiedFlipOut(GrindAbility instance, ref bool flipOut)
        {
            Vector3 up = instance.p.tf.up;
            Vector3 normalized = Vector3.ProjectOnPlane(up, Vector3.up).normalized;
            instance.p.SetRotHard(Quaternion.LookRotation(normalized));
            float num = instance.p.jumpSpeed * 0.35f;
            float num2 = instance.p.abilityTimer <= MovementSettings.RailFrameboost.Grace.Value ? MFMovementMetrics.noAbilitySpeed : instance.p.maxMoveSpeed;
            instance.p.SetVelocity(num * Vector3.up + normalized * num2);
            instance.p.ActivateAbility(instance.p.flipOutJumpAbility);
        }

        private static void OriginalJump(GrindAbility instance)
        {
            instance.p.PlayAnim(instance.jumpHash, false, false, -1f);
            Vector3 vector = instance.p.FlattenRotationHard();
            float num3 = 1f + Mathf.Clamp01(instance.p.dir.y) * 0.5f;
            if (!instance.lastPath.upwardsGrindJumpAllowed || !instance.grindLine.upwardsGrindJump)
            {
                num3 = 1f;
            }
            float num4 = instance.p.jumpSpeed + instance.p.bonusJumpSpeedGrind;
            Vector3 up = instance.p.tf.up;
            float num = ((Vector3.Dot(up, Vector3.up) > -0.1f) ? (num4 * num3) : (-instance.p.jumpSpeed * 0.5f));
            float num2 = Mathf.Min(instance.speed, instance.p.boostSpeed);
            if (instance.p.boosting)
            {
                instance.p.ActivateAbility(instance.p.boostAbility);
                instance.p.boostAbility.StartFromJumpGrindOrWallrun();
            }
            else if (instance.p.slideButtonHeld)
            {
                num *= instance.p.abilityShorthopFactor;
                instance.p.StopCurrentAbility();
                instance.p.maintainSpeedJump = true;
            }
            else
            {
                instance.p.StopCurrentAbility();
            }
            instance.p.SetVelocity(num * Vector3.up + vector * num2);
        }

        private static void ModifiedJump(GrindAbility instance)
        {
            instance.p.PlayAnim(instance.jumpHash, false, false, -1f);
            Vector3 vector = instance.p.FlattenRotationHard();
            float num3 = 1f + Mathf.Clamp01(0f) * 0.5f; //is this intentional?
            if (!instance.lastPath.upwardsGrindJumpAllowed || !instance.grindLine.upwardsGrindJump)
            {
                num3 = 1f;
            }
            float num4 = instance.p.jumpSpeed + instance.p.bonusJumpSpeedGrind;
            Vector3 up = instance.p.tf.up;
            float num = ((Vector3.Dot(up, Vector3.up) > -0.1f) ? (num4 * num3) : (-instance.p.jumpSpeed * 0.5f));
            float num2 = Mathf.Min(instance.speed, instance.p.boostSpeed);

            float slope = instance.customVelocity.y / instance.customVelocity.magnitude;
            slope = (float)Math.Round(slope, 2);
            float orientation = Vector3.Dot(instance.p.tf.up, Vector3.up);

            if (orientation >= 0.35f && Vector3.Dot(up, Vector3.up) > 0 && MovementSettings.RailSlope.Enabled.Value)
            {
                ApplyRailSlopeModifications(ref num, ref num2, slope);
            }

            ApplyBoostingOrSliding(instance, ref num);

            if (instance.p.abilityTimer <= MovementSettings.RailFrameboost.Grace.Value && MovementSettings.RailFrameboost.Enabled.Value)
            {
                ApplyFrameboost(instance, ref num2);
            }

            instance.p.SetVelocity(num * instance.p.tf.up + vector * num2);
        }

        private static void ApplyRailSlopeModifications(ref float jumpHeight, ref float jumpSpeed, float slope)
        {
            float bonusJump = slope * MovementSettings.RailSlope.SlopeJumpAmount.Value;
            float bonusSpeed = slope * -MovementSettings.RailSlope.SlopeSpeedAmount.Value;

            bonusJump *= MFMovementMetrics.AverageTotalSpeed() / 10f;
            bonusSpeed *= MFMovementMetrics.AverageTotalSpeed() / 10f;

            bonusSpeed = Mathf.Clamp(bonusSpeed, MovementSettings.RailSlope.SlopeSpeedMin.Value, MovementSettings.RailSlope.SlopeSpeedMax.Value);
            bonusJump = Mathf.Clamp(bonusJump, MovementSettings.RailSlope.SlopeJumpMin.Value, bonusJump);

            jumpHeight = Mathf.Clamp(jumpHeight + bonusJump, MovementSettings.RailSlope.SlopeJumpMin.Value, MovementSettings.RailSlope.SlopeJumpMax.Value);
            jumpSpeed = (bonusSpeed > 0f) ? MFMath.LosslessClamp(jumpSpeed, bonusSpeed, MovementSettings.RailSlope.SlopeSpeedCap.Value) : jumpSpeed + bonusSpeed;
        }

        private static void ApplyBoostingOrSliding(GrindAbility instance, ref float jumpHeight)
        {
            if (instance.p.boosting)
            {
                instance.p.ActivateAbility(instance.p.boostAbility);
                instance.p.boostAbility.StartFromJumpGrindOrWallrun();
            }
            else if (instance.p.slideButtonHeld)
            {
                jumpHeight *= instance.p.abilityShorthopFactor;
                instance.p.StopCurrentAbility();
                instance.p.maintainSpeedJump = true;
            }
            else
            {
                instance.p.StopCurrentAbility();
            }
        }

        private static void ApplyFrameboost(GrindAbility instance, ref float jumpSpeed)
        {
            float newSpeed = MFMath.LosslessClamp(jumpSpeed, MovementSettings.RailFrameboost.Amount.Value, MovementSettings.RailFrameboost.Cap.Value);
            jumpSpeed = newSpeed;
            instance.p.DoTrick(Player.TrickType.GRIND, "Frameboost", 0);
        }

        private static void CleanupJump(GrindAbility instance)
        {
            instance.p.ForceUnground(true);
            MFVariables.jumpedFromRail = true;
            MFVariables.jumpedFromRailTimer = 0.025f;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.RewardTilting))]
        [HarmonyPrefix]
        private static bool GrindAbility_RewardTilting_Prefix(Vector3 rightDir, Vector3 nextLineDir, GrindAbility __instance)
        {
            if (__instance.p.isAI || !__instance.grindLine.cornerBoost || !MovementFunkPlugin.MovementSettings.RailGeneral.ChangeEnabled.Value || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) return true;

            Vector3 currentSegmentDir = (__instance.nextNode.position - __instance.p.tf.position).normalized;
            currentSegmentDir = Vector3.ProjectOnPlane(currentSegmentDir, Vector3.up).normalized;
            Vector3 nextSegmentDir = Vector3.ProjectOnPlane(nextLineDir, Vector3.up).normalized;

            float turnAngle = Vector3.SignedAngle(currentSegmentDir, nextSegmentDir, Vector3.up);
            Side turnSide = (turnAngle < -1f) ? Side.LEFT : (turnAngle > 1f) ? Side.RIGHT : Side.NONE;

            bool isUpsideDown = Vector3.Dot(__instance.p.tf.up, Vector3.up) < 0;

            float adjustedTilt = isUpsideDown ? -__instance.grindTiltBuffer.x : __instance.grindTiltBuffer.x;
            Side playerTiltSide = (adjustedTilt < -0.25f) ? Side.LEFT : (adjustedTilt > 0.25f) ? Side.RIGHT : Side.NONE;

            if (turnSide != Side.NONE) __instance.softCornerBoost = false;

            bool isHardCorner = Mathf.Abs(turnAngle) > MovementSettings.RailGeneral.HCThresh.Value;
            bool correctInput = (playerTiltSide == turnSide);

            bool boostCorner = __instance.p.boosting && 
                               MovementFunkPlugin.MovementSettings.RailGeneral.BoostCornerEnabled.Value;

            bool cornerConditions = correctInput || (boostCorner && isHardCorner);

            if (cornerConditions && turnSide != Side.NONE)
            {
                if (isHardCorner && __instance.lastPath.hardCornerBoostsAllowed)
                {
                    __instance.p.StartScreenShake(ScreenShakeType.LIGHT, 0.2f, false);
                    __instance.p.AudioManager.PlaySfxGameplay(SfxCollectionID.GenericMovementSfx, AudioClipID.singleBoost, __instance.p.playerOneShotAudioSource, 0f);
                    __instance.p.ringParticles.Emit(1);
                    __instance.speed = MFMath.LosslessClamp(__instance.speed, MovementSettings.RailGeneral.HardAmount.Value, MovementSettings.RailGeneral.HardCap.Value);
                    __instance.p.HardCornerGrindLine(__instance.nextNode);
                    return false;
                }
                else if (__instance.lastPath.softCornerBoostsAllowed)
                {
                    __instance.softCornerBoost = true;
                    __instance.p.DoTrick(Player.TrickType.SOFT_CORNER, "Corner", 0);
                }
            }

            return false;
        }

        [HarmonyPatch(typeof(GrindAbility), nameof(GrindAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void GrindAbility_OnStopAbility_Postfix(GrindAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            RailGoon.hasGooned = false;
            RailGoon.railGoonAppllied = false;
            __instance.cooldown = MovementFunkPlugin.MovementSettings.RailGeneral.railCD.Value;
            if (MovementFunkPlugin.MovementSettings.RailGeneral.KeepVelOnExit.Value)
            {
                __instance.p.SetVelocity(__instance.customVelocity);
            }
        }
    }
}
