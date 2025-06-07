using System.Collections.Generic;
using System.Linq;

namespace MovementFunk
{
    internal class MFMisc
    {
        public static List<string> StringToList(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new List<string>();
            }

            return input.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
        }
    }
}
