using HarmonyLib;
using Reptile;

namespace MovementFunk.Patches
{
    internal static class LedgeClimbAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        private static float savedSpeed;

        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.OnStartAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_OnStartAbility_Prefix(LedgeClimbAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            savedSpeed = MFMovementMetrics.AverageTotalSpeed();
        }

        [HarmonyPatch(typeof(LedgeClimbAbility), nameof(LedgeClimbAbility.FixedUpdateAbility))]
        [HarmonyPrefix]
        private static void LedgeClimbAbility_FixedUpdateAbility_Prefix(LedgeClimbAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;
            if (__instance.p.jumpButtonNew && MovementSettings.LedgeClimbGeneral.Enabled.Value)
            {
                float newSpeed = MFMath.LosslessClamp(savedSpeed, MovementSettings.LedgeClimbGeneral.Amount.Value, MovementSettings.LedgeClimbGeneral.Cap.Value);
                __instance.p.SetVelocity(__instance.p.dir * (newSpeed));
                __instance.p.ringParticles.Emit(1);
                __instance.p.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, __instance.p.playerOneShotAudioSource, 0f);
                __instance.p.Jump();
                __instance.p.StopCurrentAbility();
            }
        }
    }
}
