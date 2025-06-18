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
            if (MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            if (MovementFunkPlugin.MovementSettings.Misc.ReturnSpeedLoading.Value)
            {
                MovementFunkPlugin.player.SetForwardSpeed(MFMovementMetrics.AverageForwardSpeed());
            }
        }
    }
}
