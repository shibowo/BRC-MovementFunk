using HarmonyLib;
using Reptile;
using MovementFunk.SpeedDisplay;

namespace MovementFunk.Patches
{
  internal static class GameplayUIPatch{
    [HarmonyPatch(typeof(GameplayUI), nameof(GameplayUI.Init))]
    [HarmonyPostfix]
    private static void GameplayUI_Init_Postfix(GameplayUI __instance){
      if (MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
      Speedometer.Init(__instance.tricksInComboLabel);
      Speedometer.InitAltTrickLabel(__instance.trickNameLabel);
    }
  }
}
