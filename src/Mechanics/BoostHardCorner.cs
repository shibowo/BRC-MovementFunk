using UnityEngine;

namespace MovementFunk.Mechanics
{
    internal class BoostHardCorner
    {
        public const float BOOST_WINDOW_BEFORE = 0.25f;
        public const float BOOST_WINDOW_AFTER = 0.25f;
        public const float COOLDOWN_DURATION = 2f;

        public static float lastBoostPressTime = float.MinValue;
        public static float lastSuccessfulBoostTime = float.MinValue;
        private static float cooldownEndTime = 0f;

        public static void Update()
        {
            CheckBoostInput();
        }

        public static void CheckBoostInput()
        {
            if (MovementFunkPlugin.player.boostButtonNew && Time.time >= cooldownEndTime)
            {
                lastBoostPressTime = Time.time;
            }
        }

        public static bool CanBoost()
        {
            return Time.time >= cooldownEndTime;
        }

        public static void StartCooldown()
        {
            cooldownEndTime = Time.time + COOLDOWN_DURATION;
        }

        public static void SuccessfulBoost()
        {
            lastSuccessfulBoostTime = Time.time;
            cooldownEndTime = 0f;
        }
    }
}
