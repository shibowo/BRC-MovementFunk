using UnityEngine;

namespace MovementPlus.Mechanics
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
            if (!railGoonAppllied && MovementPlusPlugin.ConfigSettings.RailGoon.Enabled.Value && hasGooned)
            {
                MovementPlusPlugin.player.grindAbility.speed = MPMath.LosslessClamp(Mathf.Max(MPMovementMetrics.AverageForwardSpeed(), BoostChanges.defaultBoostSpeed), MovementPlusPlugin.ConfigSettings.RailGoon.Amount.Value, MovementPlusPlugin.ConfigSettings.RailGoon.Cap.Value);
                railGoonAppllied = true;

                string name = MovementPlusPlugin.ConfigSettings.RailGoon.Name.Value;
                MPTrickManager.AddTrick(MovementPlusPlugin.ConfigSettings.RailGoon.Name.Value);
                int points = MPTrickManager.CalculateTrickValue(name, MovementPlusPlugin.ConfigSettings.RailGoon.points.Value, MovementPlusPlugin.ConfigSettings.RailGoon.pointsMin.Value, MovementPlusPlugin.ConfigSettings.Misc.listLength.Value, MovementPlusPlugin.ConfigSettings.Misc.repsToMin.Value);
                MPTrickManager.DoTrick(name, points);
            }
        }
    }
}