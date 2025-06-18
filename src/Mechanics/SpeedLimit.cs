using Reptile;

namespace MovementFunk.Mechanics
{
    internal class SpeedLimit
    {
        public static void Update()
        {
            if (MovementFunkPlugin.player.GetForwardSpeed() >= MovementFunkPlugin.MovementSettings.Misc.speedLimit.Value && MovementFunkPlugin.player.ability != MovementFunkPlugin.player.grindAbility && MovementFunkPlugin.player.ability != MovementFunkPlugin.player.wallrunAbility && MovementFunkPlugin.MovementSettings.Misc.speedLimit.Value > 0f)
            {
                float newSpeed = MovementFunkPlugin.player.GetForwardSpeed() - MovementFunkPlugin.MovementSettings.Misc.speedLimitAmount.Value * Core.dt;
                MovementFunkPlugin.player.SetForwardSpeed(newSpeed);
            }
        }
    }
}
