using HarmonyLib;
using Reptile;

namespace MovementFunk.Patches
{
    internal static class HitBounceAbilityPatch
    {
        [HarmonyPatch(typeof(HitBounceAbility), nameof(HitBounceAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void HitBounceAbility_SetState_Prefix(HitBounceAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            if (MovementFunkPlugin.MovementSettings.PopJump.Enabled.Value){
              MFVariables.savedSpeedBeforeHitBounce = MFMovementMetrics.AverageTotalSpeed();
            }
        }
    }
}
