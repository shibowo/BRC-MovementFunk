using HarmonyLib;
using Reptile;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class SpecialAirAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void SpecialAirAbility_OnStartAbility_Prefix(SpecialAirAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            if (MovementSettings.SuperTrickJump.Enabled.Value)
            {
                var num = Mathf.Max(__instance.p.GetForwardSpeed() - MovementSettings.SuperTrickJump.Threshold.Value, 0f);

                __instance.jumpSpeed = MFMath.LosslessClamp(__instance.jumpSpeed, num * MovementSettings.SuperTrickJump.Amount.Value, MovementSettings.SuperTrickJump.Cap.Value);
                __instance.duration = 0.3f;
            }
        }

        [HarmonyPatch(typeof(SpecialAirAbility), nameof(SpecialAirAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void SpecialAirAbility_OnStopAbility_Prefix(SpecialAirAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            __instance.jumpSpeed = MFVariables.defaultJumpSpeed;
        }
    }
}
