using Reptile;
using UnityEngine;

namespace MovementFunk.NewAbility
{
    public class ButtslapAbility : Ability
    {
        public ButtslapAbility(Player player) : base(player)
        {
        }

        private static MovementConfig ConfigSettings = MovementFunkPlugin.ConfigSettings;

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

        public void Activation()
        {
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (!this.p.motor.isGrounded && this.p.jumpButtonNew && ConfigSettings.Buttslap.Enabled.Value && this.p.ability == this.p.groundTrickAbility && !this.p.isJumping)
            {
                boosted = this.p.groundTrickAbility.boostTrick;
                this.p.ActivateAbility(this);
            }
        }

        public override void OnStartAbility()
        {
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
            if (!ConfigSettings.Buttslap.MultiEnabled.Value)
            {
                this.p.StopCurrentAbility();
            }
            else
            {
                this.buttslapTimer = ConfigSettings.Buttslap.Timer.Value;
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
            ConfigSettings = MovementFunkPlugin.ConfigSettings;
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
                        baseName = ConfigSettings.Buttslap.PoleBoostName.Value;
                        points = ConfigSettings.Buttslap.PoleBoostPoints.Value;
                        minPoints = ConfigSettings.Buttslap.PoleBoostPointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.PoleJumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.PoleAmount.Value;
                        cap = ConfigSettings.Buttslap.PoleCap.Value;
                        comboAmount = ConfigSettings.Buttslap.PoleComboAmount.Value;
                        break;

                    case "Surf":
                        baseName = ConfigSettings.Buttslap.SurfBoostName.Value;
                        points = ConfigSettings.Buttslap.SurfBoostPoints.Value;
                        minPoints = ConfigSettings.Buttslap.SurfBoostPointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.SurfJumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.SurfAmount.Value;
                        cap = ConfigSettings.Buttslap.SurfCap.Value;
                        comboAmount = ConfigSettings.Buttslap.SurfComboAmount.Value;
                        break;

                    default:
                        baseName = ConfigSettings.Buttslap.BoostName.Value;
                        points = ConfigSettings.Buttslap.BoostPoints.Value;
                        minPoints = ConfigSettings.Buttslap.PointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.JumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.Amount.Value;
                        cap = ConfigSettings.Buttslap.Cap.Value;
                        comboAmount = ConfigSettings.Buttslap.ComboAmount.Value;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case "Pole":
                        baseName = ConfigSettings.Buttslap.PoleName.Value;
                        points = ConfigSettings.Buttslap.PolePoints.Value;
                        minPoints = ConfigSettings.Buttslap.PoleBoostPointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.PoleJumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.PoleAmount.Value;
                        cap = ConfigSettings.Buttslap.PoleCap.Value;
                        comboAmount = ConfigSettings.Buttslap.PoleComboAmount.Value;
                        break;

                    case "Surf":
                        baseName = ConfigSettings.Buttslap.SurfName.Value;
                        points = ConfigSettings.Buttslap.SurfPoints.Value;
                        minPoints = ConfigSettings.Buttslap.SurfBoostPointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.SurfJumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.SurfAmount.Value;
                        cap = ConfigSettings.Buttslap.SurfCap.Value;
                        comboAmount = ConfigSettings.Buttslap.SurfComboAmount.Value;
                        break;

                    default:
                        baseName = ConfigSettings.Buttslap.Name.Value;
                        points = ConfigSettings.Buttslap.Points.Value;
                        minPoints = ConfigSettings.Buttslap.BoostPointsMin.Value;
                        jumpAmount = ConfigSettings.Buttslap.JumpAmount.Value;
                        forwardAmount = ConfigSettings.Buttslap.Amount.Value;
                        cap = ConfigSettings.Buttslap.Cap.Value;
                        comboAmount = ConfigSettings.Buttslap.ComboAmount.Value;
                        break;
                }
            }

            MFTrickManager.AddTrick(baseName);
            points = MFTrickManager.CalculateTrickValue(baseName, points, minPoints, ConfigSettings.Misc.listLength.Value, ConfigSettings.Misc.repsToMin.Value);

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
