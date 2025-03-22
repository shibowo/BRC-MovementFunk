using Reptile;
using UnityEngine;

namespace MovementPlus
{
    internal class MPVariables
    {
        public static bool canResetAirBoost = true;
        public static bool canResetAirDash = true;

        public static bool jumpedFromRail = false;
        public static float jumpedFromRailTimer = 0f;

        public static float defaultJumpSpeed;
        public static float savedLastSpeed;
        public static float boostAbilityTimer;
        public static string buttslapType;

        public static float savedGoon;

        public static Vector3 playerVel;

        public static void Update()
        {
            JustJumpedFromRail();
        }

        private static void JustJumpedFromRail()
        {
            if (jumpedFromRail)
            {
                MovementPlusPlugin.player.SetForwardSpeed(Mathf.Max(MPMovementMetrics.AverageForwardSpeed(), MovementPlusPlugin.player.GetForwardSpeed()));
                jumpedFromRailTimer -= Core.dt;
            }
            if (jumpedFromRailTimer <= 0f)
            {
                jumpedFromRail = false;
            }
        }
    }
}