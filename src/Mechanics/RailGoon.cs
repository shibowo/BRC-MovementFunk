using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class RailGoon
    {
        public static bool hasGooned = false;
        public static bool railGoonAppllied = false;

        public static void Update()
        {
            ApplyRailGoon();
        }

        private static void ApplyRailGoon()
        {
            if (!railGoonAppllied && MovementFunkPlugin.MovementSettings.RailGoon.Enabled.Value && hasGooned)
            {
                MovementFunkPlugin.player.grindAbility.speed = MFMath.LosslessClamp(Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), BoostChanges.defaultBoostSpeed), MovementFunkPlugin.MovementSettings.RailGoon.Amount.Value, MovementFunkPlugin.MovementSettings.RailGoon.Cap.Value);
                railGoonAppllied = true;

                string name = MovementFunkPlugin.MovementSettings.RailGoon.Name.Value;
                MFTrickManager.AddTrick(MovementFunkPlugin.MovementSettings.RailGoon.Name.Value);
                int points = MFTrickManager.CalculateTrickValue(name, MovementFunkPlugin.MovementSettings.RailGoon.points.Value, MovementFunkPlugin.MovementSettings.RailGoon.pointsMin.Value, MovementFunkPlugin.MovementSettings.Misc.listLength.Value, MovementFunkPlugin.MovementSettings.Misc.repsToMin.Value);
                MFTrickManager.DoTrick(name, points);
            }
        }
    }
}
