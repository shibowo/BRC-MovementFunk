using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class PerfectManual
    {
        public static bool slideBoost = false;
        private static float slideTimer = 0f;
        private static bool slideTimerStarted = false;

        public static void Update()
        {
            if (!MovementPlusPlugin.ConfigSettings.PerfectManual.Enabled.Value) { return; }

            if (!MovementPlusPlugin.player.IsGrounded())
            {
                slideBoost = false;
            }

            if (MovementPlusPlugin.player.slideButtonNew)
            {
                slideTimer = 0f;
                slideTimerStarted = true;
            }

            if (slideTimerStarted)
            {
                slideTimer += Time.deltaTime;
                if (MovementPlusPlugin.player.IsGrounded() && MovementPlusPlugin.player.timeGrounded <= MovementPlusPlugin.ConfigSettings.PerfectManual.Grace.Value && !slideBoost)
                {
                    DoPerfectManual();
                }

                if (slideTimer >= MovementPlusPlugin.ConfigSettings.PerfectManual.Grace.Value)
                {
                    slideTimerStarted = false;
                }
            }
        }

        private static void DoPerfectManual()
        {
            MovementPlusPlugin.player.SetForwardSpeed(MPMath.LosslessClamp(MPMovementMetrics.AverageForwardSpeed(), MovementPlusPlugin.ConfigSettings.PerfectManual.Amount.Value, MovementPlusPlugin.ConfigSettings.PerfectManual.Cap.Value));
            slideBoost = true;
            MovementPlusPlugin.player.CreateHighJumpDustEffect(MovementPlusPlugin.player.tf.up);
        }
    }
}