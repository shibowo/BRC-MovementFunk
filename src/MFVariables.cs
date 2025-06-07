using Reptile;
using UnityEngine;

namespace MovementFunk
{
    internal class MFVariables
    {
        public static bool canResetAirBoost = true;
        public static bool canResetAirDash = true;

        public static bool jumpedFromRail = false;
        public static float jumpedFromRailTimer = 0f;

        public static float defaultJumpSpeed;
        public static float savedLastSpeed; //this is wallrun specific, actually
        public static float savedSpeedBeforeHitBounce;
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
                MovementFunkPlugin.player.SetForwardSpeed(Mathf.Max(MFMovementMetrics.AverageForwardSpeed(), MovementFunkPlugin.player.GetForwardSpeed()));
                jumpedFromRailTimer -= Core.dt;
            }
            if (jumpedFromRailTimer <= 0f)
            {
                jumpedFromRail = false;
            }
        }
    }
}
