using System;
using System.Collections.Generic;

namespace Delugional.Utility
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the indexes of the flags
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int[] FlagIndexes(this Enum source)
        {
            var indexes = new List<int>();
            Array values = Enum.GetValues(source.GetType());
            for (int flagIndex = 0; flagIndex < values.Length; flagIndex++)
            {
                var flag = (Enum)values.GetValue(flagIndex);
                if (source.HasFlag(flag))
                    indexes.Add(flagIndex);
            }

            return indexes.ToArray();
        }
    }
}
