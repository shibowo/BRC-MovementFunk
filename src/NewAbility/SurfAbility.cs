using Reptile;
using System.Linq;
using UnityEngine;

namespace MovementPlus.NewAbility
{
    public class SurfAbility : Ability
    {
        public SurfAbility(Player player) : base(player)
        {
        }

        private static MyConfig ConfigSettings = MovementPlusPlugin.ConfigSettings;

        private float gravity;

        private float CDTimer = 0f;

        private bool hitDetected;
        private bool surfLite;

        public override void Init()
        {
            normalMovement = false;
            customGravity = gravity;
            allowNormalJump = true;
        }

        public void Activation()
        {
            if (CDTimer > 0f)
            {
                CDTimer -= Time.deltaTime;
            }

            if (this.p.preAbility == this.p.grindAbility && this.p.timeSinceGrinding < 0.25f)
            {
                surfLite = true;
                normalMovement = true;
            }
            else
            {
                surfLite = false;
                normalMovement = false;
            }

            bool isOnNonStableGround = this.p.IsOnNonStableGround();
            bool isSlideButtonHeld = this.p.slideButtonHeld;
            bool isNotGrinding = this.p.ability != this.p.grindAbility;
            bool isCooldownReady = CDTimer <= 0f;
            bool isNotCurrentAbility = this.p.ability != this;
            bool isGroundAngleSuitable = this.p.motor.groundAngle < 85f;
            bool isSurfEnabled = MovementPlusPlugin.ConfigSettings.NonStable.SurfEnabled.Value;
            bool isNotHandplant = MovementPlusPlugin.player.ability != MovementPlusPlugin.player.handplantAbility;

            if (isOnNonStableGround &&
                isSlideButtonHeld &&
                isNotGrinding &&
                isCooldownReady &&
                isNotCurrentAbility &&
                isGroundAngleSuitable &&
                isSurfEnabled &&
                isNotHandplant)
            {
                this.p.ActivateAbility(this);
            }
        }

        public override void OnStartAbility()
        {
            if (!surfLite)
            {
                this.customVelocity = MPMovementMetrics.AverageForwardDir() * MPMovementMetrics.AverageTotalSpeed();
                this.customVelocity.y = 0f;
                gravity = 0f;
                customGravity = gravity;
            }

            this.p.PlayAnim(Animator.StringToHash("slide"), true, false, -1f);
            this.p.characterVisual.SetBoostpackEffect(BoostpackEffectMode.BOOSTING, 1f);
        }

        public override void FixedUpdateAbility()
        {
            if (!surfLite)
            {
                CheckNonStableSurface();
                HandleNonStableGround();
                CheckGroundHit();
            }
            UpdateVelocity();
            HandleSlideButtonInput();
            HandleTrickInput();
            HandleSlidingMovement();
            DoAnim();
        }

        private void UpdateVelocity()
        {
            customVelocity = MPMovementMetrics.AverageForwardDir() * MPMovementMetrics.AverageTotalSpeed();
        }

        private void HandleSlideButtonInput()
        {
            if (!p.slideButtonHeld)
            {
                CDTimer = 0.5f;
                p.StopCurrentAbility();
            }
        }

        private void HandleNonStableGround()
        {
            if (!p.IsOnNonStableGround())
            {
                if (!hitDetected)
                {
                    p.StopCurrentAbility();
                    gravity = customGravity;
                }
                else
                {
                    gravity += 0.5f;
                    gravity = Mathf.Clamp(gravity, 0.5f, 20f);
                }
            }
        }

        private void CheckGroundHit()
        {
            if (p.motor.groundDetection.groundHit is GroundHit groundHit)
            {
                RaycastHitSomething(groundHit);
            }
        }

        private void HandleTrickInput()
        {
            if (p.AnyTrickInput())
            {
                CDTimer = 1f;
                p.ActivateAbility(p.groundTrickAbility);
            }
        }

        private void CheckNonStableSurface()
        {
            const float MaxDistance = 2.0f;

            Vector3 raycastOrigin = p.transform.position;
            int layerMask = LayerMask.GetMask("NonStableSurface", "Water", "Default");

            hitDetected = Physics.RaycastAll(raycastOrigin, Vector3.down, MaxDistance, layerMask)
                .Any(hit => IsNonStableSurface(hit));
        }

        private bool IsNonStableSurface(RaycastHit hit)
        {
            string layerName = LayerMask.LayerToName(hit.collider.gameObject.layer);

            switch (layerName)
            {
                case "NonStableSurface":
                case "Water":
                    return true;

                case "Default":
                    return IsNonstableSlope(hit);

                default:
                    return false;
            }
        }

        private bool IsNonstableSlope(RaycastHit hit)
        {
            const float SlopeThresholdMax = 89f;
            const float SlopeThresholdMin = 50f;
            float slope = Vector3.Angle(hit.normal, Vector3.up);
            return slope < SlopeThresholdMax && slope > SlopeThresholdMin;
        }

        public void DoAnim()
        {
            this.p.OrientVisualToSurfaceInAbility(18f);
        }

        private GroundHit savedGroundHit;

        private void RaycastHitSomething(GroundHit hit)
        {
            savedGroundHit = hit;
        }

        private void HandleSlidingMovement()
        {
            Vector3 groundNormal = savedGroundHit.groundNormal;

            Vector3 slidingDirection = CalculateSlidingDirection(groundNormal);

            Vector3 forwardDirection = this.p.GetVelocity().normalized;

            float slidingSpeed = CalculateSlidingSpeed(forwardDirection, slidingDirection);

            Vector3 currentVelocity = this.p.GetVelocity();

            AdjustVerticalVelocity(currentVelocity, forwardDirection);

            this.customVelocity += slidingDirection * slidingSpeed;

            this.customVelocity.y -= gravity;

            if (this.p.jumpButtonNew)
            {
                OnJump();
            }
        }

        private Vector3 CalculateSlidingDirection(Vector3 groundNormal)
        {
            Vector3 perpendicularToGround = Vector3.Cross(Vector3.down, groundNormal).normalized;
            Vector3 calculatedInputDirection = Vector3.Cross(groundNormal, perpendicularToGround);

            if (Vector3.Dot(this.p.moveInput, calculatedInputDirection) < 0)
            {
                calculatedInputDirection = -calculatedInputDirection;
            }

            return calculatedInputDirection.normalized;
        }

        private float CalculateSlidingSpeed(Vector3 forwardDirection, Vector3 slidingDirection)
        {
            return MPMath.Remap(Vector3.Dot(forwardDirection, slidingDirection), -1f, 1f, 1f, 0f);
        }

        private void AdjustVerticalVelocity(Vector3 currentVelocity, Vector3 forwardDirection)
        {
            float verticalSpeedAdjustment = Mathf.Abs(currentVelocity.y);
            verticalSpeedAdjustment = Mathf.Min(verticalSpeedAdjustment, 3f);

            if (currentVelocity.y < 0)
            {
                this.customVelocity += forwardDirection * verticalSpeedAdjustment;
            }
            else if (currentVelocity.y > 0)
            {
                this.customVelocity -= forwardDirection * (verticalSpeedAdjustment * 0.75f);
            }
        }

        public override void OnJump()
        {
        }

        public override void OnStopAbility()
        {
            this.p.RegainAirMobility();
        }
    }
}