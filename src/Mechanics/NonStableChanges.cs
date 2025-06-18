using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class NonStableChanges
    {
        public static void Update()
        {
            if (MovementFunkPlugin.player.IsOnNonStableGround() && (MovementFunkPlugin.player.ability == null || MovementFunkPlugin.player.ability == MovementFunkPlugin.player.airTrickAbility) && MovementFunkPlugin.MovementSettings.NonStable.Enabled.Value)
            {
                Vector3 velocity = MovementFunkPlugin.player.GetVelocity();

                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
                Vector3 direction = targetRotation * Vector3.forward;

                Vector3 inputDirection = MovementFunkPlugin.player.moveInput;
                Vector3 finalDirection = direction + inputDirection;

                MovementFunkPlugin.player.motor.Rotate(finalDirection, 50, true);
                MovementFunkPlugin.player.RegainAirMobility();
            }
        }
    }
}
