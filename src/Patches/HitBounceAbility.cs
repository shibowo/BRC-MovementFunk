using HarmonyLib;
using Reptile;

namespace MovementPlus.Patches
{
    internal static class HitBounceAbilityPatch
    {
        [HarmonyPatch(typeof(HitBounceAbility), nameof(HitBounceAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void HitBounceAbility_SetState_Prefix(HitBounceAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (MovementPlusPlugin.ConfigSettings.DumpsterKick.Enabled.Value){
              MPVariables.savedSpeedBeforeHitBounce = MPMovementMetrics.AverageTotalSpeed();
            }
        }
    }
}
