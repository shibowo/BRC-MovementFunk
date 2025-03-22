using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MovementPlus
{
    internal class MPTrickManager
    {
        public static Queue<string> recentTricks = new Queue<string>();
        private const int MAX_TRICKS = 100;

        public static void Update()
        {
            if (MovementPlusPlugin.player.IsComboing() == false)
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
            MovementPlusPlugin.player.currentTrickName = name;
            MovementPlusPlugin.player.currentTrickPoints = points;

            if (MovementPlusPlugin.player.tricksInCombo == 0 && MovementPlusPlugin.player.ui != null)
            {
                MovementPlusPlugin.player.ui.SetTrickingChargeBarActive(true);
            }
            MovementPlusPlugin.player.tricksInCombo++;
            if (MovementPlusPlugin.player.tricksInCombo >= 5)
            {
                float num = MovementPlusPlugin.player.gainBoostChargeCurve.Evaluate(Mathf.Min((float)MovementPlusPlugin.player.tricksInCombo, 50f) / 50f);
                MovementPlusPlugin.player.showAddCharge = num;
                MovementPlusPlugin.player.AddBoostCharge(num);
            }
            if (MovementPlusPlugin.player.scoreMultiplier == 0f)
            {
                MovementPlusPlugin.player.scoreMultiplier = 1f;
            }
            MovementPlusPlugin.player.currentTrickOnFoot = !MovementPlusPlugin.player.usingEquippedMovestyle;
            MovementPlusPlugin.player.baseScore += (float)((int)((float)MovementPlusPlugin.player.currentTrickPoints * MovementPlusPlugin.player.scoreFactor));
            MovementPlusPlugin.player.didAbilityTrick = true;
        }
    }
}