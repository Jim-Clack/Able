using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbleStrategiesServices.Support
{
    public static class SupportMethods
    {

        /// <summary>
        /// Regex to match any wildcard characters that might appear in a regex.
        /// </summary>
        private static Regex wildCards = new Regex("[^A-Za-z0123456789\\-\\–\\&\\@\\n\\r ]", RegexOptions.IgnoreCase);

        /// <summary>
        /// Does a string contain a regex wildcard?
        /// </summary>
        /// <param name="regex">To be tested</param>
        /// <returns>false if it's an alphanumeric string (+ certain chars), true if it may contain wildcards</returns>
        public static bool HasWildcard(string regex)
        {
            return wildCards.IsMatch(regex);
        }
    }
}
