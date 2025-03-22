using System.Collections.Generic;
using UnityEngine;

namespace MovementPlus.Mechanics
{
    internal class SpeedReturn
    {
        public static Queue<float> hitAngles;
        private static bool hitWall = false;

        public static void Init()
        {
            hitAngles = new Queue<float>();
        }

        public static void Update()
        {
            if (MovementPlusPlugin.player.isAI || !MovementPlusPlugin.ConfigSettings.Misc.ReturnSpeed.Value) { return; }
            PredictMovement();
            CheckIfSlow();
        }

        private static void PredictMovement()
        {
            float predictionTime = 0.1f;
            Vector3 predictedPosition = MovementPlusPlugin.player.transform.position + MovementPlusPlugin.player.GetVelocity() * predictionTime;

            Vector3 direction = (predictedPosition - MovementPlusPlugin.player.transform.position).normalized;
            float distance = Vector3.Distance(predictedPosition, MovementPlusPlugin.player.transform.position);

            int layerMask = LayerMask.GetMask("Default");

            Vector3 raycastOriginOffset = new Vector3(0, 0.15f, 0);

            RaycastHit[] hits = Physics.RaycastAll(MovementPlusPlugin.player.transform.position + raycastOriginOffset, direction, distance, layerMask);

            hitAngles.Clear();

            float angleThreshold = 25.0f;

            hitWall = false;

            foreach (RaycastHit hit in hits)
            {
                float hitAngle = Vector3.Angle(hit.normal, -direction);

                if (hitAngle < angleThreshold)
                {
                    hitAngles.Enqueue(hitAngle);
                    hitWall = true;
                }
            }
        }

        private static void CheckIfSlow()
        {
            float currentSpeed = MovementPlusPlugin.player.GetForwardSpeed();
            float averageSpeed = MPMovementMetrics.AverageForwardSpeed();
            float threshold = 30;
            float minimumSpeed = 13;

            float percentageDifference = ((averageSpeed - currentSpeed) / averageSpeed) * 100;

            bool isSpeedBelowAverage = currentSpeed < averageSpeed;
            bool isDifferenceBeyondThreshold = percentageDifference > threshold;
            bool isAbilityAllowed = MovementPlusPlugin.player.ability == MovementPlusPlugin.player.airTrickAbility ||
            MovementPlusPlugin.player.ability == MovementPlusPlugin.player.groundTrickAbility ||
            MovementPlusPlugin.player.ability == MovementPlusPlugin.player.slideAbility ||
                                    MovementPlusPlugin.player.ability == null;
            bool isAboveMinimumSpeed = currentSpeed > minimumSpeed;
            bool isOnVert = MovementPlusPlugin.player.vertShape != null;

            if (isSpeedBelowAverage && isDifferenceBeyondThreshold && !hitWall && isAbilityAllowed && isAboveMinimumSpeed && !isOnVert)
            {
                MovementPlusPlugin.player.SetForwardSpeed(averageSpeed);
            }
        }
    }
}