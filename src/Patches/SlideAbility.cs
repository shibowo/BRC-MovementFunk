using HarmonyLib;
using MovementFunk.Mechanics;
using Reptile;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class SlideAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_FixedUpdateAbility_Postfix(SlideAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            float speed = Mathf.Max(MovementSettings.SuperSlide.Speed.Value, __instance.p.GetForwardSpeed());
            if (MovementSettings.SuperSlide.Enabled.Value)
            {
                speed = MFMath.LosslessClamp(speed, MovementSettings.SuperSlide.Amount.Value, MovementSettings.SuperSlide.Cap.Value);
            }
            __instance.superSpeed = speed;

            if (MovementSettings.Misc.SlopeSlideSpeedChange.Value)
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
                        __instance.slopeSlideSpeed = forwardSpeed + (groundAngle * MovementSettings.Misc.SlopeSlideSpeedDown.Value);
                        break;

                    case 0:
                        break;

                    case 1:
                        __instance.slopeSlideSpeed = forwardSpeed - (groundAngle * MovementSettings.Misc.SlopeSlideSpeedUp.Value);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(SlideAbility), nameof(SlideAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void SlideAbility_OnStartAbility_Postfix(SlideAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            if (PerfectManual.slideBoost)
            {
                __instance.trickName = MovementFunkPlugin.MovementSettings.PerfectManual.Prefix.Value + " " + __instance.trickName;
            }
        }
    }
}
