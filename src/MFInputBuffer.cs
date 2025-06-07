using Reptile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MovementFunk
{
    internal static class MFInputBuffer
    {
        private static Dictionary<string, float> _lastPressedTimes = new Dictionary<string, float>();
        private static Player _player;

        private const float StickThreshold = 0.5f;

        private static readonly HashSet<string> TrickButtons = new HashSet<string> { "trick1", "trick2", "trick3" };

        public static void Init(Player player)
        {
            _player = player;
        }

        public static void Update()
        {
            _player = MovementFunkPlugin.player;
            if (_player == null)
                return;

            float currentTime = Time.time;

            UpdateButton("boost", _player.boostButtonNew, currentTime);
            UpdateButton("dance", _player.danceButtonNew, currentTime);
            UpdateButton("jump", _player.jumpButtonNew, currentTime);
            UpdateButton("phone", _player.phoneButtonNew, currentTime);
            UpdateButton("slide", _player.slideButtonNew, currentTime);
            UpdateButton("switchStyle", _player.switchStyleButtonNew, currentTime);
            UpdateButton("trick1", _player.trick1ButtonNew, currentTime);
            UpdateButton("trick2", _player.trick2ButtonNew, currentTime);
            UpdateButton("trick3", _player.trick3ButtonNew, currentTime);
            UpdateButton("walk", _player.walkButtonNew, currentTime);

            UpdateStickDirection(currentTime);
        }

        private static void UpdateButton(string buttonName, bool isPressed, float currentTime)
        {
            if (isPressed)
            {
                _lastPressedTimes[buttonName] = currentTime;
            }
        }

        private static void UpdateStickDirection(float currentTime)
        {
            Vector2 stickInput = _player.moveInputPlain;

            if (stickInput.magnitude >= StickThreshold)
            {
                string direction = Mathf.Abs(stickInput.x) > Mathf.Abs(stickInput.y)
                    ? (stickInput.x > 0 ? "right" : "left")
                    : (stickInput.y > 0 ? "up" : "down");

                _lastPressedTimes[direction] = currentTime;
            }
        }

        public static bool WasPressedRecently(List<string> buttonNames, float bufferDuration)
        {
            return buttonNames.All(buttonName => CheckButtonPress(buttonName, bufferDuration));
        }

        public static bool WasPressedRecentlyOrIsHeld(List<string> buttonNames, float bufferDuration)
        {
            return buttonNames.All(buttonName => CheckButtonPressOrHold(buttonName, bufferDuration));
        }

        private static bool CheckButtonPress(string buttonName, float bufferDuration)
        {
            if (buttonName == "anyTrick")
            {
                return TrickButtons.Any(trick => CheckButtonPress(trick, bufferDuration));
            }
            if (buttonName == "anyTwoTricks")
            {
                return TrickButtons.Count(trick => CheckButtonPress(trick, bufferDuration)) >= 2;
            }
            return _lastPressedTimes.TryGetValue(buttonName, out float lastPressedTime) &&
                   Time.time - lastPressedTime <= bufferDuration;
        }

        private static bool CheckButtonPressOrHold(string buttonName, float bufferDuration)
        {
            if (buttonName == "anyTrick")
            {
                return TrickButtons.Any(trick => CheckButtonPressOrHold(trick, bufferDuration));
            }
            if (buttonName == "anyTwoTricks")
            {
                return TrickButtons.Count(trick => CheckButtonPressOrHold(trick, bufferDuration)) >= 2;
            }
            return GetHeldState(buttonName) ||
                   (_lastPressedTimes.TryGetValue(buttonName, out float lastPressedTime) &&
                    Time.time - lastPressedTime <= bufferDuration);
        }

        private static bool GetHeldState(string buttonName)
        {
            if (_player == null)
                return false;

            switch (buttonName)
            {
                case "boost": return _player.boostButtonHeld;
                case "dance": return _player.danceButtonHeld;
                case "jump": return _player.jumpButtonHeld;
                case "slide": return _player.slideButtonHeld;
                case "switchStyle": return _player.switchStyleButtonHeld;
                case "trick1": return _player.trick1ButtonHeld;
                case "trick2": return _player.trick2ButtonHeld;
                case "trick3": return _player.trick3ButtonHeld;
                case "walk": return _player.walkButtonHeld;

                case "up": return _player.moveInputPlain.y >= StickThreshold;
                case "down": return _player.moveInputPlain.y <= -StickThreshold;
                case "left": return _player.moveInputPlain.x <= -StickThreshold;
                case "right": return _player.moveInputPlain.x >= StickThreshold;

                default: return false;
            }
        }
    }
}
