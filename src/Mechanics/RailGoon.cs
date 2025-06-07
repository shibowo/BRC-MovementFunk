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
            if (!railGoonAppllied && MovementFunkPlugin.ConfigSettings.RailGoon.Enabled.Value && hasGooned)
            {
                MovementFunkPlugin.player.grindAbility.speed = MFMath.LosslessClamp(Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), BoostChanges.defaultBoostSpeed), MovementFunkPlugin.ConfigSettings.RailGoon.Amount.Value, MovementFunkPlugin.ConfigSettings.RailGoon.Cap.Value);
                railGoonAppllied = true;

                string name = MovementFunkPlugin.ConfigSettings.RailGoon.Name.Value;
                MFTrickManager.AddTrick(MovementFunkPlugin.ConfigSettings.RailGoon.Name.Value);
                int points = MFTrickManager.CalculateTrickValue(name, MovementFunkPlugin.ConfigSettings.RailGoon.points.Value, MovementFunkPlugin.ConfigSettings.RailGoon.pointsMin.Value, MovementFunkPlugin.ConfigSettings.Misc.listLength.Value, MovementFunkPlugin.ConfigSettings.Misc.repsToMin.Value);
                MFTrickManager.DoTrick(name, points);
            }
        }
    }
}
