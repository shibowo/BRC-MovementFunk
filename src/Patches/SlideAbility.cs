using HarmonyLib;
using MovementFunk.Mechanics;
using Reptile;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class SlideAbilityPatch
    {
        private static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_FixedUpdateAbility_Postfix(SlideAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            float speed = Mathf.Max(ConfigSettings.SuperSlide.Speed.Value, __instance.p.GetForwardSpeed());
            if (ConfigSettings.SuperSlide.Enabled.Value)
            {
                speed = MFMath.LosslessClamp(speed, ConfigSettings.SuperSlide.Amount.Value, ConfigSettings.SuperSlide.Cap.Value);
            }
            __instance.superSpeed = speed;

            if (ConfigSettings.Misc.SlopeSlideSpeedChange.Value)
            {
                Vector3 velocity = __instance.p.GetVelocity();
                float forwardSpeed = __instance.p.GetForwardSpeed();

                bool isMovingDown = velocity.y < 0;

                float groundAngle = __instance.p.motor.groundAngle;

                int slopeCase;

                if (groundAngle == 0)
                {
                    slopeCase = 0;
                }
                else if (isMovingDown)
                {
                    slopeCase = -1;
                }
                else
                {
                    slopeCase = 1;
                }

                switch (slopeCase)
                {
                    case -1:
                        __instance.slopeSlideSpeed = forwardSpeed + (groundAngle * ConfigSettings.Misc.SlopeSlideSpeedDown.Value);
                        break;

                    case 0:
                        break;

                    case 1:
                        __instance.slopeSlideSpeed = forwardSpeed - (groundAngle * ConfigSettings.Misc.SlopeSlideSpeedUp.Value);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_OnStartAbility_Postfix(SlideAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (PerfectManual.slideBoost)
            {
                __instance.trickName = MovementFunkPlugin.ConfigSettings.PerfectManual.Prefix.Value + " " + __instance.trickName;
            }
        }
    }
}
