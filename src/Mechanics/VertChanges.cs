using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class VertChanges
    {
        public static float defaultVertMaxSpeed;
        public static float defaultVertTopJumpSpeed;

        public static void Update()
        {
            if (MovementFunkPlugin.player.ability != MovementFunkPlugin.player.vertAbility)
            {
                HandleVertAbility();
            }
        }

        private static void HandleVertAbility()
        {
            if (MovementFunkPlugin.ConfigSettings.VertGeneral.Enabled.Value)
            {
                UpdateVertBottomExitSpeed();
            }

            if (MovementFunkPlugin.ConfigSettings.VertGeneral.JumpEnabled.Value)
            {
                UpdateVertTopJumpSpeed();
            }
        }

        private static void UpdateVertBottomExitSpeed()
        {
            float playerTotal = MFMovementMetrics.AverageTotalSpeed();
            var x = playerTotal + MovementFunkPlugin.ConfigSettings.VertGeneral.ExitSpeed.Value;
            x = Mathf.Min(x, MovementFunkPlugin.ConfigSettings.VertGeneral.ExitSpeedCap.Value);
            MovementFunkPlugin.player.vertMaxSpeed = Mathf.Max(defaultVertMaxSpeed, MovementFunkPlugin.player.GetTotalSpeed());
            MovementFunkPlugin.player.vertBottomExitSpeed = x;
        }

        private static void UpdateVertTopJumpSpeed()
        {
            float playerTotal = MFMovementMetrics.AverageTotalSpeed();
            var x = Mathf.Max(defaultVertTopJumpSpeed, playerTotal * MovementFunkPlugin.ConfigSettings.VertGeneral.JumpStrength.Value);
            x = Mathf.Min(MovementFunkPlugin.ConfigSettings.VertGeneral.JumpCap.Value, x);
            MovementFunkPlugin.player.vertTopJumpSpeed = x;
        }
    }
}
