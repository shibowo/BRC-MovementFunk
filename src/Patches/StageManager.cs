using HarmonyLib;
using Reptile;

namespace MovementFunk.Patches
{
    internal static class StageManagerPatch
    {
        [HarmonyPatch(typeof(StageManager), nameof(StageManager.DoStagePostInitialization))]
        [HarmonyPrefix]
        private static void StageManager_OnStageInitialized_Prefix(StageManager __instance)
        {
            if (MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (MovementFunkPlugin.ConfigSettings.Misc.ReturnSpeedLoading.Value)
            {
                MovementFunkPlugin.player.SetForwardSpeed(MFMovementMetrics.AverageForwardSpeed());
            }
        }
    }
}
