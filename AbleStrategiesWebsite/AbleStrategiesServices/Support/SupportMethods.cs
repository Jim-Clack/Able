using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Shorten a man-readable input string to no more than maxChars characters.
        /// </summary>
        /// <param name="instring">to be shortened</param>
        /// <param name="maxChars">Max desired length, minimum 8, defaults to 12</param>
        /// <returns>the abbreviated string</returns>
        public static string Shorten(string instring, int maxChars = 12)
        {
            if (instring == null)
            {
                return "(null)";
            }
            maxChars = Math.Max(8, maxChars);
            if (instring.Length < maxChars)
            {
                return instring;
            }
            int maxReplacements = 1 + instring.Length - maxChars;
            Regex regex = new Regex("[^bcdfghjklmnpqrstvwxyz0123456789]", 
                RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string outString = regex.Replace(instring, "", maxReplacements); // remove vowels, whitespace, and punctuation
            if (outString.Length > maxChars) // remove middle portion of string
            {
                outString = outString.Substring(0, maxChars - 4) + "\'" + outString.Substring(outString.Length - 3);
            }
            return outString;
        }

    }

}
