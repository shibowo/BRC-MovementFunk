using HarmonyLib;
using MovementFunk.Mechanics;
using MovementFunk.NewAbility;
using MovementFunk.SpeedDisplay;
using Reptile;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using static MovementFunk.MovementConfig;

namespace MovementFunk.Patches
{
    internal static class PlayerPatch
    {
        private static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

        public static ButtslapAbility buttslapAbility;
        public static SurfAbility surfAbility;

        [HarmonyPatch(typeof(Player), nameof(Player.Init))]
        [HarmonyPostfix]
        private static void Player_Init_Postfix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (MovementFunkPlugin.player == null)
            {
                MovementFunkPlugin.player = __instance;
                BoostChanges.defaultBoostSpeed = __instance.normalBoostSpeed;
                VertChanges.defaultVertMaxSpeed = __instance.vertMaxSpeed;
                VertChanges.defaultVertTopJumpSpeed = __instance.vertTopJumpSpeed;
                MFVariables.defaultJumpSpeed = __instance.specialAirAbility.jumpSpeed;
                __instance.motor.maxFallSpeed = ConfigSettings.Misc.maxFallSpeed.Value;

                __instance.wallrunAbility.lastSpeed = MFVariables.savedLastSpeed;
                buttslapAbility = new ButtslapAbility(__instance);
                surfAbility = new SurfAbility(__instance);

                if (ConfigSettings.Misc.collisionChangeEnabled.Value)
                {
                    var bodies = __instance.GetComponentsInChildren<Rigidbody>();
                    foreach (var body in bodies)
                    {
                        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdatePlayer))]
        [HarmonyPostfix]
        private static void Player_FixedUpdatePlayer_Postfix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }

            buttslapAbility.Activation();
            surfAbility.Activation();
            if (!__instance.IsComboing())
            {
                __instance.ClearMultipliersDone();
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
        public static class Player_Jump_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 2].Calls(AccessTools.Method(typeof(Player), "get_maxMoveSpeed")) &&
                        codes[i + 3].opcode == OpCodes.Ldc_R4 && (float)codes[i + 3].operand == 2f &&
                        codes[i + 4].opcode == OpCodes.Add &&
                        codes[i + 5].Calls(AccessTools.Method(typeof(Player), "SetForwardSpeed")))
                    {
                        codes.RemoveRange(i, 6);

                        codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player_Jump_Transpiler), nameof(JumpPadSetForwardSpeed))));

                        break;
                    }
                }

                return codes;
            }

            private static void JumpPadSetForwardSpeed(Player player)
            {
                if (player.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value)
                {
                    player.SetForwardSpeed(player.maxMoveSpeed + 2f);
                }
                float speed = Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), player.maxMoveSpeed, player.GetForwardSpeed() + 2f);
                player.SetForwardSpeed(speed);
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.Jump))]
        [HarmonyPostfix]
        private static void Player_Jump_Postfix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (Fastfall.timeSinceLastFastFall < MovementFunkPlugin.ConfigSettings.WaveDash.grace.Value && !__instance.slideButtonHeld)
            {
                float speed;
                string name;
                int points;

                if (__instance.boostButtonHeld)
                {
                    speed = MFMovementMetrics.AverageForwardSpeed() + MovementFunkPlugin.ConfigSettings.WaveDash.BoostSpeed.Value;
                    name = MovementFunkPlugin.ConfigSettings.WaveDash.BoostName.Value;
                    MFTrickManager.AddTrick(name);
                    points = MFTrickManager.CalculateTrickValue(name, ConfigSettings.WaveDash.BoostPoints.Value, ConfigSettings.WaveDash.BoostPointsMin.Value, ConfigSettings.Misc.listLength.Value, ConfigSettings.Misc.repsToMin.Value);
                    MFTrickManager.DoTrick(name, points);
                    __instance.StopCurrentAbility();
                }
                else
                {
                    speed = MFMovementMetrics.AverageForwardSpeed() + MovementFunkPlugin.ConfigSettings.WaveDash.NormalSpeed.Value;
                    name = MovementFunkPlugin.ConfigSettings.WaveDash.NormalName.Value;
                    MFTrickManager.AddTrick(name);
                    points = MFTrickManager.CalculateTrickValue(name, ConfigSettings.WaveDash.NormalPoints.Value, ConfigSettings.WaveDash.NormalPointsMin.Value, ConfigSettings.Misc.listLength.Value, ConfigSettings.Misc.repsToMin.Value);
                    MFTrickManager.DoTrick(name, points);
                }

                __instance.SetForwardSpeed(speed);
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.OnLanded))]
        public static class Player_OnLanded_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 2].Calls(AccessTools.Method(typeof(Player), "get_maxMoveSpeed")) &&
                        codes[i + 3].Calls(AccessTools.Method(typeof(Player), "SetSpeedFlat")))
                    {
                        codes.RemoveRange(i, 4);
                        codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player_OnLanded_Transpiler), nameof(HardLanding))));
                        break;
                    }
                }
                return codes;
            }

            private static void HardLanding(Player player)
            {
                var config = MovementFunkPlugin.ConfigSettings.Misc;

                if (player.isAI || !config.HardLandingEnabled.Value || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value)
                {
                    player.SetSpeedFlat(player.maxMoveSpeed);
                    return;
                }

                bool isHardFall = MFMovementMetrics.lastAirTime >= config.HardFallTime.Value;

                if (!isHardFall)
                {
                    return;
                }

                switch (config.HardLandingMode.Value)
                {
                    case HardLandingType.Off:
                        break;

                    case HardLandingType.OnlyFeet:
                        if (player.moveStyle == MoveStyle.ON_FOOT)
                        {
                            player.SetSpeedFlat(player.maxMoveSpeed);
                        }
                        break;

                    case HardLandingType.OnlyMovestyle:
                        if (player.moveStyle != MoveStyle.ON_FOOT)
                        {
                            player.SetSpeedFlat(player.maxMoveSpeed);
                        }
                        break;

                    case HardLandingType.FeetAndMovestyle:
                        player.SetSpeedFlat(player.maxMoveSpeed);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.LandCombo))]
        [HarmonyPrefix]
        private static bool Player_LandCombo_Prefix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return true; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (__instance.comboTimeOutTimer <= 0)
            {
                if (WorldHandler.instance.currentEncounter != null)
                {
                    __instance.ClearMultipliersDone();
                }
                return true;
            }
            if (__instance.boosting && ConfigSettings.ComboGeneral.BoostEnabled.Value)
            {
                return false;
            }
            if (ConfigSettings.ComboGeneral.NoAbilityEnabled.Value)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ClearMultipliersDone))]
        [HarmonyPrefix]
        private static bool Player_ClearMultipliersDone_Prefix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return true; }
            if (__instance.comboTimeOutTimer <= 0 || !__instance.IsComboing())
            {
                return true;
            }
            return false;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.RegainAirMobility))]
        [HarmonyPostfix]
        private static void Player_RegainAirMobility_Postfix(Player __instance)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            Fastfall.canFastFall = true;
            MFVariables.canResetAirBoost = true;
            MFVariables.canResetAirDash = true;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.FixedUpdateAbilities))]
        public static class Player_FixedUpdateAbilities_Transpiler
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (int i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].Calls(AccessTools.Method(typeof(Player), "LandCombo")))
                    {
                        codes[i + 1] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Player_FixedUpdateAbilities_Transpiler), nameof(NoAbilityComboTimeout)));
                        break;
                    }
                }

                return codes;
            }

            private static void NoAbilityComboTimeout(Player player)
            {
                if (player.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value)
                {
                    player.LandCombo();
                    return;
                }
                ConfigSettings = MovementFunkPlugin.ConfigSettings;
                if (player.IsComboing() && Fastfall.timeSinceLastFastFall <= ConfigSettings.WaveDash.grace.Value)
                {
                    return;
                }
                if (player.IsComboing() && ConfigSettings.ComboGeneral.NoAbilityEnabled.Value && !player.boosting)
                {
                    float num = Mathf.Min(player.GetForwardSpeed() / player.boostSpeed, 0.95f);
                    player.DoComboTimeOut(Mathf.Max(Core.dt * (1f - num), Core.dt / 2f) * ConfigSettings.ComboGeneral.NoAbilityTimeout.Value);
                    return;
                }
                player.LandCombo();
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.DoTrick))]
        [HarmonyPostfix]
        private static void Player_DoTrick_Postfix(Player __instance, Player.TrickType type, string trickName = "", int trickNum = 0)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
            if (__instance.ability != __instance.slideAbility)
            {
                MFTrickManager.AddTrick(type.ToString());
            }
        }

        [HarmonyPatch(typeof(Player), nameof(Player.JumpIsAllowed))]
        [HarmonyPrefix]
        private static bool Player_JumpIsAllowed_Prefix(Player __instance, ref bool __result)
        {
            if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return true; }
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            bool abilityAllowsJump = __instance.ability == null || __instance.ability.allowNormalJump;
            bool jumpNotConsumed = !__instance.jumpConsumed;
            bool noVertShape = __instance.vertShape == null;
            bool isGroundedOrRecentlyGrounded = __instance.IsGrounded() ||
                                                __instance.timeSinceLastAbleToJump <= __instance.JumpPostGroundingGraceTime;
            bool buttslapCheck = __instance.ability == __instance.groundTrickAbility && !__instance.IsGrounded() && ConfigSettings.Buttslap.Enabled.Value;

            __result = jumpNotConsumed && abilityAllowsJump && noVertShape && isGroundedOrRecentlyGrounded && !buttslapCheck;

            return false;
        }

        [HarmonyPatch(typeof(Player), nameof(Player.ChargeAndSpeedDisplayUpdate))]
        [HarmonyPostfix]
        private static void ChargeAndSpeedDisplayUpdate_Postfix(Player __instance){
          if (__instance.isAI || MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
          Speedometer.Update();
        }
        [HarmonyPatch(typeof(GameplayUI), nameof(GameplayUI.Init))]
        [HarmonyPostfix]
        private static void Player_GameplayUI_Init_Postfix(GameplayUI __instance){
          if (MovementFunkPlugin.ConfigSettings.Misc.DisablePatch.Value) { return; }
          Speedometer.Init(__instance.tricksInComboLabel);
        }
    }
}
