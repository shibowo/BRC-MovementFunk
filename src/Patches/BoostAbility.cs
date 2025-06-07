using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class BoostAbilityPatch
    {
        private static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

        private static float defaultBoostSpeed;
        private static float preY;

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_OnStartAbility_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }

            MFVariables.boostAbilityTimer = MovementFunkPlugin.ConfigSettings.BoostGeneral.decc.Value;
            preY = __instance.p.GetVelocity().y;
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.Init))]
        [HarmonyPostfix]
        private static void BoostAbility_Init_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            __instance.decc = MovementFunkPlugin.ConfigSettings.BoostGeneral.decc.Value;
            defaultBoostSpeed = __instance.p.normalBoostSpeed;
        }

        [HarmonyPatch(typeof(BoostAbility), nameof(BoostAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void BoostAbility_FixedUpdateAbility_Postfix(BoostAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;

            MFVariables.boostAbilityTimer += Core.dt;

            if (__instance.p.IsGrounded() && __instance.p.IsComboing() && ConfigSettings.ComboGeneral.BoostEnabled.Value)
            {
                __instance.p.DoComboTimeOut((Core.dt / 2f) * ConfigSettings.ComboGeneral.BoostTimeout.Value);
            }

            RetainYBoost(__instance);

            if (ConfigSettings.BoostGeneral.StartEnabled.Value)
            {
                if (__instance.state == BoostAbility.State.START_BOOST)
                {
                    float speed = (__instance.p.ability == __instance.p.grindAbility) ? MFMovementMetrics.AverageForwardSpeed() : MFMovementMetrics.AverageTotalSpeed();
                    float highestSpeed = Mathf.Max(defaultBoostSpeed, speed);
                    float newSpeed = MFMath.LosslessClamp(highestSpeed, ConfigSettings.BoostGeneral.StartAmount.Value, ConfigSettings.BoostGeneral.StartCap.Value);
                    __instance.p.normalBoostSpeed = newSpeed;
                    return;
                }
            }
            if (ConfigSettings.BoostGeneral.TotalSpeedEnabled.Value)
            {
                float newSpeed = MFMath.LosslessClamp(MFMovementMetrics.AverageForwardSpeed(), MFMovementMetrics.AverageTotalSpeed() - MFMovementMetrics.AverageForwardSpeed(), ConfigSettings.BoostGeneral.TotalSpeedCap.Value);
                __instance.p.normalBoostSpeed = newSpeed;
            }
        }

        private static void RetainYBoost(BoostAbility __instance)
        {
            if (!__instance.p.IsGrounded())
            {
                if (__instance.state == BoostAbility.State.START_BOOST && MovementFunkPlugin.ConfigSettings.BoostGeneral.Force0.Value)
                {
                    __instance.p.motor.SetVelocityYOneTime(0f);
                    return;
                }

                if (MovementFunkPlugin.ConfigSettings.BoostGeneral.RetainY.Value)
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
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            preY = __instance.p.jumpSpeed;
            if (__instance.p.IsComboing())
            {
                __instance.p.DoComboTimeOut(ConfigSettings.ComboGeneral.BoostJumpAmount.Value);
            }
        }
    }
}
