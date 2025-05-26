using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class VertAbilityPatch
    {
        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.OnStartAbility))]
        [HarmonyPostfix]
        private static void VertAbility_OnStartAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            __instance.p.airDashAbility.haveAirDash = false;
        }

        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.OnStopAbility))]
        [HarmonyPostfix]
        private static void VertAbility_OnStopAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            __instance.p.airDashAbility.haveAirDash = true;
        }

        [HarmonyPatch(typeof(VertAbility), nameof(VertAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void VertAbility_FixedUpdateAbility_Postfix(VertAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (__instance.p.jumpButtonNew)
            {
                __instance.p.StopCurrentAbility();

                List<string> backButton = MPMisc.StringToList("down");
                bool isBackHeld = MPInputBuffer.WasPressedRecentlyOrIsHeld(backButton, 0.1f);

                if (!isBackHeld)
                {
                    __instance.p.SetRotation(__instance.p.tf.rotation * Quaternion.Euler(0, 180, 0));
                }

                Vector3 newVelocity = MPMovementMetrics.AverageForwardDir() * MPMovementMetrics.AverageTotalSpeed();

                if (__instance.p.slideButtonHeld)
                {
                    __instance.p.SetVelocity(newVelocity);
                }
            }
        }
    }
}