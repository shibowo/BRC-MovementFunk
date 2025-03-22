using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class VertChanges
    {
        public static float defaultVertMaxSpeed;
        public static float defaultVertTopJumpSpeed;

        public static void Update()
        {
            if (MovementPlusPlugin.player.ability != MovementPlusPlugin.player.vertAbility)
            {
                HandleVertAbility();
            }
        }

        private static void HandleVertAbility()
        {
            if (MovementPlusPlugin.ConfigSettings.VertGeneral.Enabled.Value)
            {
                UpdateVertBottomExitSpeed();
            }

            if (MovementPlusPlugin.ConfigSettings.VertGeneral.JumpEnabled.Value)
            {
                UpdateVertTopJumpSpeed();
            }
        }

        private static void UpdateVertBottomExitSpeed()
        {
            float playerTotal = MPMovementMetrics.AverageTotalSpeed();
            var x = playerTotal + MovementPlusPlugin.ConfigSettings.VertGeneral.ExitSpeed.Value;
            x = Mathf.Min(x, MovementPlusPlugin.ConfigSettings.VertGeneral.ExitSpeedCap.Value);
            MovementPlusPlugin.player.vertMaxSpeed = Mathf.Max(defaultVertMaxSpeed, MovementPlusPlugin.player.GetTotalSpeed());
            MovementPlusPlugin.player.vertBottomExitSpeed = x;
        }

        private static void UpdateVertTopJumpSpeed()
        {
            float playerTotal = MPMovementMetrics.AverageTotalSpeed();
            var x = Mathf.Max(defaultVertTopJumpSpeed, playerTotal * MovementPlusPlugin.ConfigSettings.VertGeneral.JumpStrength.Value);
            x = Mathf.Min(MovementPlusPlugin.ConfigSettings.VertGeneral.JumpCap.Value, x);
            MovementPlusPlugin.player.vertTopJumpSpeed = x;
        }
    }
}