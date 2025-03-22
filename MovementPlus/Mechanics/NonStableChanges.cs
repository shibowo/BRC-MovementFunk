using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class NonStableChanges
    {
        public static void Update()
        {
            if (MovementPlusPlugin.player.IsOnNonStableGround() && (MovementPlusPlugin.player.ability == null || MovementPlusPlugin.player.ability == MovementPlusPlugin.player.airTrickAbility) && MovementPlusPlugin.ConfigSettings.NonStable.Enabled.Value)
            {
                Vector3 velocity = MovementPlusPlugin.player.GetVelocity();

                Quaternion targetRotation = Quaternion.LookRotation(velocity.normalized);
                Vector3 direction = targetRotation * Vector3.forward;

                Vector3 inputDirection = MovementPlusPlugin.player.moveInput;
                Vector3 finalDirection = direction + inputDirection;

                MovementPlusPlugin.player.motor.Rotate(finalDirection, 50, true);
                MovementPlusPlugin.player.RegainAirMobility();
            }
        }
    }
}