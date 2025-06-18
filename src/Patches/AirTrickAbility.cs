using HarmonyLib;
using Reptile;

namespace MovementFunk.Patches
{
    internal static class AirTrickAbilityPatch
    {
        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        [HarmonyPatch(typeof(AirTrickAbility), nameof(AirTrickAbility.SetupBoostTrick))]
        [HarmonyPrefix]
        private static void AirTrickAbility_SetupBoostTrick_Prefix(AirDashAbility __instance)
        {
            if (__instance.p.isAI || MovementFunkPlugin.MovementSettings.Misc.DisablePatch.Value) { return; }
            MovementSettings = MovementFunkPlugin.MovementSettings;

            switch (MovementSettings.BoostGeneral.BoostDashReturnType.Value)
            {
                case MovementConfig.BoostReturnType.Once:
                    if (MFVariables.canResetAirDash && !__instance.p.airDashAbility.haveAirDash)
                    {
                        __instance.p.airDashAbility.haveAirDash = true;
                        MFVariables.canResetAirDash = false;
                    }
                    break;

                case MovementConfig.BoostReturnType.Always:
                    __instance.p.airDashAbility.haveAirDash = true;
                    break;

                case MovementConfig.BoostReturnType.Disabled:
                    break;
            }

            if (__instance.p.preAbility == __instance.p.boostAbility && MFVariables.boostAbilityTimer <= 0.25f && MovementSettings.BoostGeneral.NoBoostLossTrick.Value)
            {
                __instance.p.boostAbility.haveAirStartBoost = true;
                return;
            }

            switch (MovementSettings.BoostGeneral.BoostReturnType.Value)
            {
                case MovementConfig.BoostReturnType.Once:
                    if (MFVariables.canResetAirBoost && !__instance.p.boostAbility.haveAirStartBoost)
                    {
                        __instance.p.boostAbility.haveAirStartBoost = true;
                        MFVariables.canResetAirBoost = false;
                    }
                    break;

                case MovementConfig.BoostReturnType.Always:
                    __instance.p.boostAbility.haveAirStartBoost = true;
                    break;

                case MovementConfig.BoostReturnType.Disabled:
                    break;
            }
        }
    }
}
