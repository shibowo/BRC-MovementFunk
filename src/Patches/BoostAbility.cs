using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class BoostAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        private static float defaultBoostSpeed;
        private static float preY;

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_OnStartAbility_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }

            MFVariables.boostAbilityTimer = MovementFunkPlugin.MovementSettings.BoostGeneral.decc.Value;
            preY = __instance.p.GetVelocity().y;
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.Init))]
        [HarmonyPostfix]
        private static void BoostAbility_Init_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            __instance.decc = MovementFunkPlugin.MovementSettings.BoostGeneral.decc.Value;
            defaultBoostSpeed = __instance.p.normalBoostSpeed;
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_FixedUpdateAbility_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;

            MFVariables.boostAbilityTimer += Core.dt;

            if (__instance.p.IsGrounded() && __instance.p.IsComboing() && MovementSettings.ComboGeneral.BoostEnabled.Value)
            {
                __instance.p.DoComboTimeOut((Core.dt / 2f) * MovementSettings.ComboGeneral.BoostTimeout.Value);
            }

            RetainYBoost(__instance);

            if (MovementSettings.BoostGeneral.StartEnabled.Value)
            {
                if (__instance.state == BoostAbility.State.START_BOOST)
                {
                    float speed = (__instance.p.ability == __instance.p.grindAbility) ? MFMovementMetrics.AverageForwardSpeed() : MFMovementMetrics.AverageTotalSpeed();
                    float highestSpeed = Mathf.Max(defaultBoostSpeed, speed);
                    float newSpeed = MFMath.LosslessClamp(highestSpeed, MovementSettings.BoostGeneral.StartAmount.Value, MovementSettings.BoostGeneral.StartCap.Value);
                    __instance.p.normalBoostSpeed = newSpeed;
                    return;
                }
            }
            if (MovementSettings.BoostGeneral.TotalSpeedEnabled.Value)
            {
                float newSpeed = MFMath.LosslessClamp(MFMovementMetrics.AverageForwardSpeed(), MFMovementMetrics.AverageTotalSpeed() - MFMovementMetrics.AverageForwardSpeed(), MovementSettings.BoostGeneral.TotalSpeedCap.Value);
                __instance.p.normalBoostSpeed = newSpeed;
            }
        }

        private static void RetainYBoost(BoostAbility __instance)
        {
            if (!__instance.p.IsGrounded())
            {
                if (__instance.state == BoostAbility.State.START_BOOST && MovementFunkPlugin.MovementSettings.BoostGeneral.Force0.Value)
                {
                    __instance.p.motor.SetVelocityYOneTime(0f);
                    return;
                }

                if (MovementFunkPlugin.MovementSettings.BoostGeneral.RetainY.Value)
                {
                    __instance.p.motor.SetVelocityYOneTime(preY);
                    preY -= __instance.p.motor.gravity * Core.dt;
                }
            }
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnJump))]
        [HarmonyPrefix]
        private static void BoostAbility_OnJump_PreFix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            preY = __instance.p.jumpSpeed;
            if (__instance.p.IsComboing())
            {
                __instance.p.DoComboTimeOut(MovementSettings.ComboGeneral.BoostJumpAmount.Value);
            }
        }
    }
}
