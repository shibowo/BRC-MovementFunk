using HarmonyLib;
using Reptile;

namespace MovementPlus.Patches
{
    internal static class LedgeClimbAbilityPatch
    {
        private static MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        private static float savedSpeed;

        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_OnStartAbility_Prefix(LedgeClimbAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            savedSpeed = MPMovementMetrics.AverageTotalSpeed();
        }

        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_FixedUpdateAbility_Prefix(LedgeClimbAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementPlusPlugin.ConfigSettings;
            if (__instance.p.jumpButtonNew && ConfigSettings.LedgeClimbGeneral.Enabled.Value)
            {
                float newSpeed = MPMath.LosslessClamp(savedSpeed, ConfigSettings.LedgeClimbGeneral.Amount.Value, ConfigSettings.LedgeClimbGeneral.Cap.Value);
                __instance.p.SetVelocity(__instance.p.dir * (newSpeed));
                __instance.p.ringParticles.Emit(1);
                __instance.p.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, __instance.p.playerOneShotAudioSource, 0f);
                __instance.p.Jump();
                __instance.p.StopCurrentAbility();
            }
        }
    }
}