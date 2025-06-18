using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace MovementFunk.Patches
{
    internal static class VertAbilityPatch
    {
        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void VertAbility_OnStartAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            __instance.p.airDashAbility.haveAirDash = false;
        }

        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void VertAbility_OnStopAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            __instance.p.airDashAbility.haveAirDash = true;
        }

        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void VertAbility_FixedUpdateAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            if (__instance.p.jumpButtonNew)
            {
                __instance.p.StopCurrentAbility();

                List<string> backButton = MFMisc.StringToList("down");
                bool isBackHeld = MFInputBuffer.WasPressedRecentlyOrIsHeld(backButton, 0.1f);

                if (!isBackHeld)
                {
                    __instance.p.SetRotation(__instance.p.tf.rotation * Quaternion.Euler(0, 180, 0));
                }

                Vector3 newVelocity = MFMovementMetrics.AverageForwardDir() * MFMovementMetrics.AverageTotalSpeed();

                if (__instance.p.slideButtonHeld)
                {
                    __instance.p.SetVelocity(newVelocity);
                }
            }
        }
    }
}
