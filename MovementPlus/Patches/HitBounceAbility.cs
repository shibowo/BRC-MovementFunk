using HarmonyLib;
using Reptile;

namespace MovementPlus.Patches
{
    internal static class HitBounceAbilityPatch
    {
        private static float preSpeed;

        [HarmonyPatch(typeof(HitBounceAbility), nameof(HitBounceAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void HitBounceAbility_SetState_Prefix(HitBounceAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            preSpeed = MPMovementMetrics.AverageTotalSpeed();
        }

        [HarmonyPatch(typeof(HitBounceAbility), nameof(HitBounceAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        private static void HitBounceAbility_FixedUpdateAbility_Postfix(HitBounceAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (__instance.p.jumpButtonNew)
            {
                __instance.p.StopCurrentAbility();
                __instance.p.motor.SetVelocityYOneTime(preSpeed);
            }
        }
    }
}