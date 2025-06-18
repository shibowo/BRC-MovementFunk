using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class PerfectManual
    {
        public static bool slideBoost = false;
        private static float slideTimer = 0f;
        private static bool slideTimerStarted = false;

        public static void Update()
        {
            if (!MovementFunkPlugin.MovementSettings.PerfectManual.Enabled.Value) { return; }

            if (!MovementFunkPlugin.player.IsGrounded())
            {
                slideBoost = false;
            }

            if (MovementFunkPlugin.player.slideButtonNew)
            {
                slideTimer = 0f;
                slideTimerStarted = true;
            }

            if (slideTimerStarted)
            {
                slideTimer += Time.deltaTime;
                if (MovementFunkPlugin.player.IsGrounded() && MovementFunkPlugin.player.timeGrounded <= MovementFunkPlugin.MovementSettings.PerfectManual.Grace.Value && !slideBoost)
                {
                    DoPerfectManual();
                }

                if (slideTimer >= MovementFunkPlugin.MovementSettings.PerfectManual.Grace.Value)
                {
                    slideTimerStarted = false;
                }
            }
        }

        private static void DoPerfectManual()
        {
            MovementFunkPlugin.player.SetForwardSpeed(MFMath.LosslessClamp(MFMovementMetrics.AverageForwardSpeed(), MovementFunkPlugin.MovementSettings.PerfectManual.Amount.Value, MovementFunkPlugin.MovementSettings.PerfectManual.Cap.Value));
            slideBoost = true;
            MovementFunkPlugin.player.CreateHighJumpDustEffect(MovementFunkPlugin.player.tf.up);
        }
    }
}
