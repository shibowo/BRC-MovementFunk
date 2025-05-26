using HarmonyLib;
using Reptile;

namespace MovementPlus.Patches
{
    internal static class AirTrickAbilityPatch
    {
        private static MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        [HarmonyPatch(typeof(AirTrickAbility), nameof(AirTrickAbility.SetupBoostTrick))]
        [HarmonyPrefix]
        private static void AirTrickAbility_SetupBoostTrick_Prefix(AirDashAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementPlusPlugin.ConfigSettings;

            switch (ConfigSettings.BoostGeneral.BoostDashReturnType.Value)
            {
                case MyConfig.BoostReturnType.Once:
                    if (MPVariables.canResetAirDash && !__instance.p.airDashAbility.haveAirDash)
                    {
                        __instance.p.airDashAbility.haveAirDash = true;
                        MPVariables.canResetAirDash = false;
                    }
                    break;

                case MyConfig.BoostReturnType.Always:
                    __instance.p.airDashAbility.haveAirDash = true;
                    break;

                case MyConfig.BoostReturnType.Disabled:
                    break;
            }

            if (__instance.p.preAbility == __instance.p.boostAbility && MPVariables.boostAbilityTimer <= 0.25f && ConfigSettings.BoostGeneral.NoBoostLossTrick.Value)
            {
                __instance.p.boostAbility.haveAirStartBoost = true;
                return;
            }

            switch (ConfigSettings.BoostGeneral.BoostReturnType.Value)
            {
                case MyConfig.BoostReturnType.Once:
                    if (MPVariables.canResetAirBoost && !__instance.p.boostAbility.haveAirStartBoost)
                    {
                        __instance.p.boostAbility.haveAirStartBoost = true;
                        MPVariables.canResetAirBoost = false;
                    }
                    break;

                case MyConfig.BoostReturnType.Always:
                    __instance.p.boostAbility.haveAirStartBoost = true;
                    break;

                case MyConfig.BoostReturnType.Disabled:
                    break;
            }
        }
    }
}