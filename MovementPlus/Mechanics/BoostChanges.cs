using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class BoostChanges
    {
        public static float defaultBoostSpeed;

        public static void Update()
        {
            if (MovementPlusPlugin.ConfigSettings.BoostGeneral.InfiniteBoost.Value)
            {
                MovementPlusPlugin.player.boostCharge = MovementPlusPlugin.player.maxBoostCharge;
            }
            if (RailGoon.hasGooned)
            {
                RailGoon.Update();
                return;
            }

            if (MovementPlusPlugin.player.ability == MovementPlusPlugin.player.wallrunAbility)
            {
                return;
            }

            if (MovementPlusPlugin.player.ability != MovementPlusPlugin.player.boostAbility && MovementPlusPlugin.ConfigSettings.BoostGeneral.SpeedScale.Value)
            {
                MovementPlusPlugin.player.normalBoostSpeed = Mathf.Max(defaultBoostSpeed, MPMovementMetrics.AverageForwardSpeed(), MovementPlusPlugin.player.GetForwardSpeed());
            }
        }
    }
}