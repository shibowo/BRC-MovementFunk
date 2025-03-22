using Reptile;

namespace MovementPlus.Mechanics
{
    internal class SpeedLimit
    {
        public static void Update()
        {
            if (MovementPlusPlugin.player.GetForwardSpeed() >= MovementPlusPlugin.ConfigSettings.Misc.speedLimit.Value && MovementPlusPlugin.player.ability != MovementPlusPlugin.player.grindAbility && MovementPlusPlugin.player.ability != MovementPlusPlugin.player.wallrunAbility && MovementPlusPlugin.ConfigSettings.Misc.speedLimit.Value > 0f)
            {
                float newSpeed = MovementPlusPlugin.player.GetForwardSpeed() - MovementPlusPlugin.ConfigSettings.Misc.speedLimitAmount.Value * Core.dt;
                MovementPlusPlugin.player.SetForwardSpeed(newSpeed);
            }
        }
    }
}