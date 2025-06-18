using Reptile;
using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class Fastfall
    {
        public static float timeSinceLastFastFall = 0f;
        public static bool canFastFall;

        public static void Update()
        {
            Player player = MovementFunkPlugin.player;
            bool isSlideButtonPressed = player.slideButtonNew;
            bool isNotSortaGrounded = !player.TreatPlayerAsSortaGrounded();
            bool isFallingOrOffConfig = player.motor.velocity.y <= 0f || !MovementFunkPlugin.MovementSettings.FastFall.FallEnabled.Value;
            bool isFastFallEnabled = MovementFunkPlugin.MovementSettings.FastFall.Enabled.Value;
            bool isNotHandplant = player.ability != player.handplantAbility;

            bool shouldFastFall = isSlideButtonPressed &&
                                  isNotSortaGrounded &&
                                  isFallingOrOffConfig &&
                                  canFastFall &&
                                  isFastFallEnabled &&
                                  isNotHandplant;

            if (shouldFastFall)
            {
                float fastFallAmount = MovementFunkPlugin.MovementSettings.FastFall.Amount.Value;
                float newVelocityY = Mathf.Min(player.motor.velocity.y + fastFallAmount, fastFallAmount);

                player.motor.SetVelocityYOneTime(newVelocityY);
                player.ringParticles.Emit(1);
                player.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, player.playerOneShotAudioSource, 0f);
                canFastFall = false;

                timeSinceLastFastFall = 0f;

                player.grindAbility.cooldown = 0f;

                if (player.ability == player.boostAbility && MovementFunkPlugin.MovementSettings.FastFall.CancelBoost.Value)
                {
                    player.StopCurrentAbility();
                }
            }
            else if (player.ability != player.groundTrickAbility)
            {
                timeSinceLastFastFall += Time.deltaTime;
            }
        }
    }
}
