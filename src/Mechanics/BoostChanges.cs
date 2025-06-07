using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class BoostChanges
    {
        public static float defaultBoostSpeed;

        public static void Update()
        {
            if (MovementFunkPlugin.ConfigSettings.BoostGeneral.InfiniteBoost.Value)
            {
                MovementFunkPlugin.player.boostCharge = MovementFunkPlugin.player.maxBoostCharge;
            }
            if (RailGoon.hasGooned)
            {
                RailGoon.Update();
                return;
            }

            if (MovementFunkPlugin.player.ability == MovementFunkPlugin.player.wallrunAbility)
            {
                return;
            }

            if (MovementFunkPlugin.player.ability != MovementFunkPlugin.player.boostAbility && MovementFunkPlugin.ConfigSettings.BoostGeneral.SpeedScale.Value)
            {
                MovementFunkPlugin.player.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, MFMovementMetrics.AverageForwardSpeed(), MovementFunkPlugin.player.GetForwardSpeed());
            }
        }
    }
}
