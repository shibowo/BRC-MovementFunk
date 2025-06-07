using UnityEngine;

namespace MovementFunk
{
    internal class MFMath
    {
        public static float Remap(float val, float in1, float in2, float out1, float out2)
        {
            return out1 + (val - in1) * (out2 - out1) / (in2 - in1);
        }

        public static float TableCurve(float a, float b, float c, float x)
        {
            return (((a + b) * x) / (b + x) + c);
        }

        public static float LosslessClamp(float input, float toAdd, float cap)
        {
            float output = input;
            float potentialOutput = output + toAdd;
            if (cap < 0)
            {
                return potentialOutput;
            }

            output = Mathf.Min(potentialOutput, cap);
            output = Mathf.Max(output, input);

            return output;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (min >= 0)
            {
                value = Mathf.Max(value, min);
            }

            if (max >= 0)
            {
                value = Mathf.Min(value, max);
            }

            return value;
        }
    }
}
