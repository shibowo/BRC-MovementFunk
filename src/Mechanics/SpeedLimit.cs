using Reptile;

namespace MovementFunk.Mechanics
{
    internal class SpeedLimit
    {
        public static void Update()
        {
            if (MovementFunkPlugin.player.GetForwardSpeed() >= MovementFunkPlugin.ConfigSettings.Misc.speedLimit.Value && MovementFunkPlugin.player.ability != MovementFunkPlugin.player.grindAbility && MovementFunkPlugin.player.ability != MovementFunkPlugin.player.wallrunAbility && MovementFunkPlugin.ConfigSettings.Misc.speedLimit.Value > 0f)
            {
                float newSpeed = MovementFunkPlugin.player.GetForwardSpeed() - MovementFunkPlugin.ConfigSettings.Misc.speedLimitAmount.Value * Core.dt;
                MovementFunkPlugin.player.SetForwardSpeed(newSpeed);
            }
        }
    }
}
