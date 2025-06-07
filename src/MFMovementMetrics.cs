using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace MovementFunk
{
    internal class MFMovementMetrics
    {
        public static Queue<float> forwardSpeeds;
        public static Queue<float> totalSpeeds;
        public static Queue<Vector3> forwardDirs;
        public static float averageSpeedTimer = 0f;
        public static float averageForwardTimer = 0f;
        public static float noAbilitySpeed;
        public static float timeInAir = 0f;
        public static float lastAirTime = 0f;
        public static bool wasInAir;

        public static void Init()
        {
            forwardSpeeds = new Queue<float>();
            totalSpeeds = new Queue<float>();
            forwardDirs = new Queue<Vector3>();
            averageSpeedTimer = 0f;
        }

        public static void Update()
        {
            SaveSpeed();
            TimeInAir();
            LogSpeed(MovementFunkPlugin.ConfigSettings.Misc.averageSpeedTimer.Value);
            LogForward(0.032f);
        }

        public static void LogSpeed(float time)
        {
            if (MovementFunkPlugin.player.CheckVert())
            {
                return;
            }
            forwardSpeeds.Enqueue(MovementFunkPlugin.player.GetForwardSpeed());
            totalSpeeds.Enqueue(MovementFunkPlugin.player.GetTotalSpeed());
            if (averageSpeedTimer >= time)
            {
                forwardSpeeds.Dequeue();
                totalSpeeds.Dequeue();
            }
            else
            {
                averageSpeedTimer += Core.dt;
            }
        }

        public static float AverageForwardSpeed()
        {
            if (forwardSpeeds.Count == 0)
                return 0f;

            float sum = 0f;
            float maxSpeed = float.MinValue;
            foreach (float speed in forwardSpeeds)
            {
                sum += speed;
                if (speed > maxSpeed)
                    maxSpeed = speed;
            }
            float average = sum / forwardSpeeds.Count;

            return CalculateSpeedBasedOnMode(average, maxSpeed);
        }

        public static float AverageTotalSpeed()
        {
            if (totalSpeeds.Count == 0)
                return 0f;

            float sum = 0f;
            float maxSpeed = float.MinValue;
            foreach (float speed in totalSpeeds)
            {
                sum += speed;
                if (speed > maxSpeed)
                    maxSpeed = speed;
            }
            float average = sum / totalSpeeds.Count;

            return CalculateSpeedBasedOnMode(average, maxSpeed);
        }

        private static float CalculateSpeedBasedOnMode(float average, float maxSpeed)
        {
            switch (MovementFunkPlugin.ConfigSettings.Misc.averageSpeedMode.Value)
            {
                case MovementConfig.AverageSpeedMode.Average:
                    return average;

                case MovementConfig.AverageSpeedMode.Max:
                    return maxSpeed;

                case MovementConfig.AverageSpeedMode.BlendMaxAverage:
                    float bias = MovementFunkPlugin.ConfigSettings.Misc.averageSpeedBias.Value;

                    float blendFactor = (maxSpeed - average) / (maxSpeed + float.Epsilon);

                    float finalBlendFactor = Mathf.Lerp(blendFactor, bias, 0.5f);

                    return Mathf.Lerp(average, maxSpeed, finalBlendFactor);

                default:
                    return average;
            }
        }

        private static void LogForward(float time)
        {
            forwardDirs.Enqueue(MovementFunkPlugin.player.tf.forward);
            if (averageForwardTimer >= time)
            {
                forwardDirs.Dequeue();
            }
            else
            {
                averageForwardTimer += Core.dt;
            }
        }

        public static Vector3 AverageForwardDir()
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 forwardDir in forwardDirs)
            {
                sum += forwardDir;
            }
            return sum / forwardDirs.Count;
        }

        private static void SaveSpeed()
        {
            noAbilitySpeed = (MovementFunkPlugin.player.ability == MovementFunkPlugin.player.grindAbility || MovementFunkPlugin.player.ability == MovementFunkPlugin.player.handplantAbility)
            ? Mathf.Max(noAbilitySpeed, MovementFunkPlugin.player.grindAbility.speed)
            : MovementFunkPlugin.player.GetForwardSpeed();
        }

        private static void TimeInAir()
        {
            bool isGrounded = MovementFunkPlugin.player.TreatPlayerAsSortaGrounded();

            if (isGrounded)
            {
                if (wasInAir)
                {
                    lastAirTime = timeInAir;
                }
                timeInAir = 0f;
                wasInAir = false;
            }
            else
            {
                timeInAir += Core.dt;
                wasInAir = true;
            }
        }
    }
}
