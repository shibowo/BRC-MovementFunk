using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace MovementPlus.Patches
{
    internal static class HandplantAbilityPatch
    {
        private static MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        [HarmonyPatch(typeof(HandplantAbility), nameof(HandplantAbility.FixedUpdateAbility))]
        public static class HandplantAbility_FixedUpdateAbility_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldc_R4 && (float)codes[i].operand == 0.12f)
                    {
                        codes[i].operand = 0f;
                        break;
                    }
                }

                return codes;
            }
        }

        [HarmonyPatch(typeof(HandplantAbility), nameof(HandplantAbility.FixedUpdateAbility))]
        [HarmonyPostfix]
        public static void HandplantAbility_FixedUpdateAbility_Postfix(HandplantAbility __instance)
        {
            if (__instance.p.isAI || MovementPlusPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementPlusPlugin.ConfigSettings;
            if (__instance.p.abilityTimer <= 0.12f + ConfigSettings.RailFrameboost.Grace.Value)
            {
                __instance.p.SetForwardSpeed(Mathf.Max(MPMovementMetrics.noAbilitySpeed, __instance.p.GetForwardSpeed()));
            }
            if (__instance.p.AnyTrickInput() && ConfigSettings.Handplant.Enabled.Value)
            {
                float jumpAmount = __instance.p.jumpSpeed;
                if (__instance.p.abilityTimer <= 0.12f + ConfigSettings.RailFrameboost.Grace.Value)
                {
                    jumpAmount = Mathf.Max(jumpAmount, MPMovementMetrics.noAbilitySpeed * ConfigSettings.Handplant.Strength.Value);
                }
                __instance.p.ActivateAbility(__instance.p.groundTrickAbility);
                __instance.p.motor.SetVelocityYOneTime(jumpAmount);
            }
        }
    }
}