using HarmonyLib;
using Reptile;

namespace MovementPlus.Patches
{
    internal static class StageManagerPatch
    {
        [HarmonyPatch(typeof(StageManager), nameof(StageManager.DoStagePostInitialization))]
        [HarmonyPrefix]
        private static void StageManager_OnStageInitialized_Prefix(StageManager __instance)
        {
            if (MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (MovementPlusPlugin.ConfigSettings.Misc.ReturnSpeedLoading.Value)
            {
                MovementPlusPlugin.player.SetForwardSpeed(MPMovementMetrics.AverageForwardSpeed());
            }
        }
    }
}