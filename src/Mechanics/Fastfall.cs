using Reptile;
using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class Fastfall
    {
        public static float timeSinceLastFastFall = 0f;
        public static bool canFastFall;

        public static void Update()
        {
            Player player = MovementPlusPlugin.player;
            bool isSlideButtonPressed = player.slideButtonNew;
            bool isNotSortaGrounded = !player.TreatPlayerAsSortaGrounded();
            bool isFallingOrOffConfig = player.motor.velocity.y <= 0f || !MovementPlusPlugin.ConfigSettings.FastFall.FallEnabled.Value;
            bool isFastFallEnabled = MovementPlusPlugin.ConfigSettings.FastFall.Enabled.Value;
            bool isNotHandplant = player.ability != player.handplantAbility;

            bool shouldFastFall = isSlideButtonPressed &&
                                  isNotSortaGrounded &&
                                  isFallingOrOffConfig &&
                                  canFastFall &&
                                  isFastFallEnabled &&
                                  isNotHandplant;

            if (shouldFastFall)
            {
                float fastFallAmount = MovementPlusPlugin.ConfigSettings.FastFall.Amount.Value;
                float newVelocityY = Mathf.Min(player.motor.velocity.y + fastFallAmount, fastFallAmount);

                player.motor.SetVelocityYOneTime(newVelocityY);
                player.ringParticles.Emit(1);
                player.AudioManager.PlaySfxGameplay(global::Reptile.SfxCollectionID.GenericMovementSfx, global::Reptile.AudioClipID.singleBoost, player.playerOneShotAudioSource, 0f);
                canFastFall = false;

                timeSinceLastFastFall = 0f;

                player.grindAbility.cooldown = 0f;

                if (player.ability == player.boostAbility && MovementPlusPlugin.ConfigSettings.FastFall.CancelBoost.Value)
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