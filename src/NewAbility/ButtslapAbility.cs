using Reptile;
using UnityEngine;

namespace MovementFunk.NewAbility
{
    public class ButtslapAbility : Ability
    {
        public ButtslapAbility(Player player) : base(player)
        {
        }

        private static MovementConfig MovementSettings = MovementFunkPlugin.MovementSettings;

        private float buttslapTimer;
        private bool boosted;
        private int buttslapAmount;
        private string type;

        public override void Init()
        {
            allowNormalJump = false;
            normalRotation = true;
            buttslapTimer = 0f;
        }

        public bool Activation()
        {
            MovementSettings = MovementFunkPlugin.MovementSettings;
            if (!this.p.motor.isGrounded && this.p.jumpButtonNew && MovementSettings.Buttslap.Enabled.Value && this.p.ability == this.p.groundTrickAbility && !this.p.isJumping)
            {
                boosted = this.p.groundTrickAbility.boostTrick;
                this.p.ActivateAbility(this);
                return true;
            }
            return false;
        }

        public override void OnStartAbility()
        {
            MovementSettings = MovementFunkPlugin.MovementSettings;
            if (!MovementSettings.Buttslap.MultiEnabled.Value)
            {
                this.p.StopCurrentAbility();
            }
            else
            {
                this.buttslapTimer = MovementSettings.Buttslap.Timer.Value;
                this.buttslapAmount = 0;
            }
            type = MFVariables.buttslapType;
            this.PerformButtslap();
        }

        public override void FixedUpdateAbility()
        {
            if (buttslapTimer <= 0f)
            {
                this.p.StopCurrentAbility();
                return;
            }

            if (this.p.AnyTrickInput() && this.p.abilityTimer >= 0.1f)
            {
                this.p.ActivateAbility(this.p.airTrickAbility);
            }

            this.buttslapTimer -= Core.dt;

            this.p.SetVisualRotLocal0();

            if (this.p.jumpButtonNew && !this.p.TreatPlayerAsSortaGrounded())
            {
                this.PerformButtslap();
            }
            else if (this.p.TreatPlayerAsSortaGrounded())
            {
                buttslapTimer = 0f;
            }
        }

        private void PerformButtslap()
        {
            this.p.audioManager.PlayVoice(ref this.p.currentVoicePriority, this.p.character, AudioClipID.VoiceJump, this.p.playerGameplayVoicesAudioSource, VoicePriority.MOVEMENT);
            this.p.PlayAnim(this.p.jumpHash, true, true, -1f);

            this.p.isJumping = true;
            this.p.jumpRequested = false;
            this.p.jumpConsumed = true;
            this.p.jumpedThisFrame = true;
            this.p.timeSinceLastJump = 0f;

            PerformTrick(type, boosted);
        }

        private void PerformTrick(string type, bool boosted)
        {
            MovementSettings = MovementFunkPlugin.MovementSettings;
            this.buttslapAmount++;
            string baseName;
            int points;
            int minPoints;
            float jumpAmount;
            float forwardAmount;
            float cap;
            float comboAmount;

            if (boosted)
            {
                switch (type)
                {
                    case "Pole":
                        baseName = MovementSettings.Buttslap.PoleBoostName.Value;
                        points = MovementSettings.Buttslap.PoleBoostPoints.Value;
                        minPoints = MovementSettings.Buttslap.PoleBoostPointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.PoleJumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.PoleAmount.Value;
                        cap = MovementSettings.Buttslap.PoleCap.Value;
                        comboAmount = MovementSettings.Buttslap.PoleComboAmount.Value;
                        break;

                    case "Surf":
                        baseName = MovementSettings.Buttslap.SurfBoostName.Value;
                        points = MovementSettings.Buttslap.SurfBoostPoints.Value;
                        minPoints = MovementSettings.Buttslap.SurfBoostPointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.SurfJumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.SurfAmount.Value;
                        cap = MovementSettings.Buttslap.SurfCap.Value;
                        comboAmount = MovementSettings.Buttslap.SurfComboAmount.Value;
                        break;

                    default:
                        baseName = MovementSettings.Buttslap.BoostName.Value;
                        points = MovementSettings.Buttslap.BoostPoints.Value;
                        minPoints = MovementSettings.Buttslap.PointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.JumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.Amount.Value;
                        cap = MovementSettings.Buttslap.Cap.Value;
                        comboAmount = MovementSettings.Buttslap.ComboAmount.Value;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case "Pole":
                        baseName = MovementSettings.Buttslap.PoleName.Value;
                        points = MovementSettings.Buttslap.PolePoints.Value;
                        minPoints = MovementSettings.Buttslap.PoleBoostPointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.PoleJumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.PoleAmount.Value;
                        cap = MovementSettings.Buttslap.PoleCap.Value;
                        comboAmount = MovementSettings.Buttslap.PoleComboAmount.Value;
                        break;

                    case "Surf":
                        baseName = MovementSettings.Buttslap.SurfName.Value;
                        points = MovementSettings.Buttslap.SurfPoints.Value;
                        minPoints = MovementSettings.Buttslap.SurfBoostPointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.SurfJumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.SurfAmount.Value;
                        cap = MovementSettings.Buttslap.SurfCap.Value;
                        comboAmount = MovementSettings.Buttslap.SurfComboAmount.Value;
                        break;

                    default:
                        baseName = MovementSettings.Buttslap.Name.Value;
                        points = MovementSettings.Buttslap.Points.Value;
                        minPoints = MovementSettings.Buttslap.BoostPointsMin.Value;
                        jumpAmount = MovementSettings.Buttslap.JumpAmount.Value;
                        forwardAmount = MovementSettings.Buttslap.Amount.Value;
                        cap = MovementSettings.Buttslap.Cap.Value;
                        comboAmount = MovementSettings.Buttslap.ComboAmount.Value;
                        break;
                }
            }

            MFTrickManager.AddTrick(baseName);
            points = MFTrickManager.CalculateTrickValue(baseName, points, minPoints, MovementSettings.Misc.listLength.Value, MovementSettings.Misc.repsToMin.Value);

            string name = $"{baseName} x {this.buttslapAmount}";
            this.p.currentTrickName = name;
            MFTrickManager.DoTrick(name, points);

            jumpAmount += Mathf.Max(this.p.GetVelocity().y, 0f);
            this.p.DoJumpEffects(this.p.motor.groundNormalVisual * -1f);
            this.p.motor.SetVelocityYOneTime(jumpAmount);
            this.p.SetForwardSpeed(MFMath.LosslessClamp(this.p.GetForwardSpeed(), forwardAmount, cap));
            this.p.DoComboTimeOut(comboAmount);
        }
    }
}
