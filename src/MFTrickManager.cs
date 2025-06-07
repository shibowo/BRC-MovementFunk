using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MovementFunk
{
    internal class MFTrickManager
    {
        public static Queue<string> recentTricks = new Queue<string>();
        private const int MAX_TRICKS = 100;

        public static void Update()
        {
            if (MovementFunkPlugin.player.IsComboing() == false)
            {
                recentTricks.Clear();
            }
        }

        public static void AddTrick(string trickName)
        {
            recentTricks.Enqueue(trickName);

            while (recentTricks.Count > MAX_TRICKS)
                recentTricks.Dequeue();
        }

        public static int CalculateTrickValue(string trickName, int baseValue, int minValue, int scoringWindow, int repetitionsToMin)
        {
            scoringWindow = Math.Min(scoringWindow, recentTricks.Count);

            var scoringTricks = recentTricks.Reverse().Take(scoringWindow);

            int repetitions = scoringTricks.Count(t => t == trickName);

            int scoreDecrease = (baseValue - minValue) / (repetitionsToMin - 1);

            int score = Math.Max(baseValue - (repetitions - 1) * scoreDecrease, minValue);

            return score;
        }

        public static void DoTrick(string name, int points)
        {
            MovementFunkPlugin.player.currentTrickName = name;
            MovementFunkPlugin.player.currentTrickPoints = points;

            if (MovementFunkPlugin.player.tricksInCombo == 0 && MovementFunkPlugin.player.ui != null)
            {
                MovementFunkPlugin.player.ui.SetTrickingChargeBarActive(true);
            }
            MovementFunkPlugin.player.tricksInCombo++;
            if (MovementFunkPlugin.player.tricksInCombo >= 5)
            {
                float num = MovementFunkPlugin.player.gainBoostChargeCurve.Evaluate(Mathf.Min((float)MovementFunkPlugin.player.tricksInCombo, 50f) / 50f);
                MovementFunkPlugin.player.showAddCharge = num;
                MovementFunkPlugin.player.AddBoostCharge(num);
            }
            if (MovementFunkPlugin.player.scoreMultiplier == 0f)
            {
                MovementFunkPlugin.player.scoreMultiplier = 1f;
            }
            MovementFunkPlugin.player.currentTrickOnFoot = !MovementFunkPlugin.player.usingEquippedMovestyle;
            MovementFunkPlugin.player.baseScore += (float)((int)((float)MovementFunkPlugin.player.currentTrickPoints * MovementFunkPlugin.player.scoreFactor));
            MovementFunkPlugin.player.didAbilityTrick = true;
        }
    }
}
