using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AbleCheckbook.Logic
{
    public class BankRegex
    {
        private const String CAPTURE_HASHCODE = "\\#c";
        private const String BASE64_HASHCODE = "\\#6";
        private const String MATCH_QUOTED = "[^\"]*";

        private String[] captures = new String[] { "(0)", "(1)", "(2)", "(3)", "(4)", "(5)", "(6)", "(7)", "(8)", "(9)" };
        private string statement = "";
        private BankConfiguration config = null;
        private string errorMessage = "";
        private string userId = null;
        private string password = null;
        private string routing = null;
        private string account = null;
        private DateTime startDate = DateTime.Now;
        private DateTime endDate = DateTime.Now;

        public string Statement { get => statement; set => statement = value; }
        public string ErrorMessage { get => errorMessage; }

        public BankRegex(BankConfiguration config, string userId, string password, string routing, string account, DateTime startDate, DateTime endDate)
        {
            this.config = config;
            this.userId = userId;
            this.routing = routing;
            this.account = account;
            this.password = password;
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public String ExpandRequest(string request)
        {
            errorMessage = "";

            return "";
        }

        public String ExpandResponse(string response)
        {
            errorMessage = "";

            return "";
        }

        public String GetCapture(int index)
        {
            return captures[index];
        }

        public String ProcessRequest(string inString)
        {
            inString = ReplaceBackslashes(inString);
            int length = inString.Length;
            Byte[] inBuffer = Encoding.ASCII.GetBytes(inString);
            StringBuilder outBuffer = new StringBuilder();
            errorMessage = "";

            for (int startColumn = 0; startColumn < length; ++startColumn)
            {
                if (startColumn < length - 2 && inBuffer[startColumn] == (Byte)'\\' && inBuffer[startColumn + 1] == (Byte)'#')
                {
                    startColumn += ExpandHashCode(inString, length, inBuffer, outBuffer, startColumn);
                }
                else
                {
                    outBuffer.Append((char)inBuffer[startColumn]);
                }
                if (errorMessage.Length > 0)
                {
                    outBuffer.Clear();
                    outBuffer.Append(errorMessage);
                    break;
                }
            }
            return outBuffer.ToString();
        }

        public String ProcessResponse(string expected, string inString)
        {
            errorMessage = "";
            expected = ReplaceBackslashes(expected);
            if (expected.Length < 1)
            {
                return inString;
            }
            if (expected.StartsWith(CAPTURE_HASHCODE))
            {
                errorMessage = "Cannot start pattern with capture# " + expected;
                return errorMessage;
            }
            if (inString.Length < 1)
            {
                errorMessage = "Missing response for " + expected;
                return errorMessage;
            }
            inString = DecodeMimeFromBase64(inString);
            if (errorMessage.Length == 0)
            {
                inString = DecodeBase64Data(expected, inString);
            }
            if (errorMessage.Length == 0)
            {
                ValidateAndCapture(expected, inString);
            }
            if (errorMessage.Length > 0)
            {
                return errorMessage;
            }
            return inString;
        }

        private void ValidateAndCapture(string expected, string inString)
        {

            // base64 was already done, now match what WAS base64 data
            string expected2 = expected.Replace(BASE64_HASHCODE, MATCH_QUOTED);
            // disassemble substrings from expected
            string[] matchStrings = expected2.Split(new String[] { CAPTURE_HASHCODE }, StringSplitOptions.RemoveEmptyEntries);
            int[] captureNumbers = new int[matchStrings.Length];
            Array.Clear(captureNumbers, 0, captureNumbers.Length);
            for (int substringNumber = 0; substringNumber < matchStrings.Length; ++substringNumber)
            {
                string matchString = matchStrings[substringNumber];
                if (substringNumber > 0 && matchString.Length > 1)
                {
                    captureNumbers[substringNumber] = ((Byte)matchString[0]) % 16;
                    matchStrings[substringNumber] = matchString.Substring(1); // strip capture number
                }
            }
            // assemble substrings from inString
            int startColumn = 0;
            int prevMatchEndIndex = 0;
            for (int substringNumber = 0; substringNumber < matchStrings.Length; ++substringNumber)
            {
                Regex regExp = new Regex(matchStrings[substringNumber], RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
                Match match = regExp.Match(inString, startColumn);
                if (!match.Success)
                {
                    errorMessage = "No match to " + matchStrings[substringNumber] + " on " + inString + " from col " + startColumn;
                    break;
                }
                if (substringNumber > 0)
                {
                    int captureLength = match.Index - prevMatchEndIndex;
                    int captureIndex = match.Index - captureLength;
                    captures[captureNumbers[substringNumber]] = inString.Substring(captureIndex, captureLength);
                }
                prevMatchEndIndex = match.Index + match.Length;
                startColumn = match.Index;
            }
        }

        private string DecodeMimeFromBase64(string inString)
        {
            // mimeType:application/json, fileName and fileContent fields?
            // inline vs attachment
            // content disposition values

            // TODO

            return inString;
        }

        /// <summary>
        /// Backup plan for base64 conversion, clumsy and restrictive, but sometimes handy
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="inString"></param>
        /// <returns>Updated inString</returns>
        private string DecodeBase64Data(string expected, string inString)
        {
            int indexInExpected = expected.IndexOf(BASE64_HASHCODE);
            if (indexInExpected <= 0)
            {
                return inString;
            }
            string prefix = "";
            int prefixIndex = 0;
            while (prefix.Length < 20)
            {
                if (--indexInExpected < 0)
                {
                    break;
                }
                prefix = expected[indexInExpected] + prefix;
                if (prefix.StartsWith("#") || prefix.StartsWith("\\"))
                {
                    prefix = prefix.Substring(3);
                    break;
                }
            }
            if (prefix.Length < 3)
            {
                errorMessage = "Need a longer fixed prefix before \\#6 to help identify it";
            }
            else if ((prefixIndex = inString.IndexOf(prefix)) < 1)
            {
                errorMessage = "Cannot find base64 data per given \\#6 pattern in " + expected;
            }
            else if (inString.IndexOf(prefix, prefixIndex + prefix.Length) > 1)
            {
                errorMessage = "Multiple matches for \\#6 base64 prefix " + prefix + " found in response";
            }
            if (errorMessage.Length == 0)
            {
                StringBuilder outBuffer = new StringBuilder();
                int base64Index = prefixIndex + prefix.Length;
                int base64Length = ConvertFromBase64(inString, base64Index, outBuffer);
                if (base64Length > 0)
                {
                    inString = inString.Substring(0, base64Index) + outBuffer.ToString() + inString.Substring(base64Index + base64Length);
                }
            }
            return inString;
        }

        private string ReplaceBackslashes(string inString)
        {
            return inString.Replace("\\n", "\x0a").Replace("\\r", "\x0d").Replace("\\t", "\x09").Replace("\\#q", "\x22")
                .Replace("\\#n", "\x0a").Replace("\\#r", "\x0d").Replace("\\#t", "\x09").Replace("\\\\", "\\");
        }

        private int ExpandHashCode(string inString, int length, byte[] inBuffer, StringBuilder outBuffer, int startColumn)
        {
            int skipCharCount = 2;
            switch (inBuffer[startColumn + 2])
            {
                case (Byte)'\\':
                    skipCharCount = 1;
                    break;
                case (Byte)'#':
                    outBuffer.Append('#');
                    break;
                case (Byte)'q':
                    outBuffer.Append('\"');
                    break;
                case (Byte)'r':
                    outBuffer.Append('\x0D');
                    break;
                case (Byte)'n':
                    outBuffer.Append('\x0A');
                    break;
                case (Byte)'t':
                    outBuffer.Append('\x09');
                    break;
                case (Byte)'u':
                    outBuffer.Append(userId);
                    break;
                case (Byte)'p':
                    outBuffer.Append(password);
                    break;
                case (Byte)'h':
                    Byte[] basicAuth = System.Text.Encoding.UTF8.GetBytes(userId + ":" + password);
                    string base64Auth = "Basic " + System.Convert.ToBase64String(basicAuth);
                    outBuffer.Append(base64Auth);
                    break;
                case (Byte)'b':
                    outBuffer.Append(routing);
                    break;
                case (Byte)'a':
                    outBuffer.Append(account);
                    break;
                case (Byte)'s':
                    outBuffer.Append(FormatDate(startDate));
                    break;
                case (Byte)'e':
                    outBuffer.Append(FormatDate(endDate));
                    break;
                case (Byte)'&':
                    outBuffer.Append(config.AccountRequestAddToHeader);
                    break;
                case (Byte)'x':
                    DateTime exclusiveEndDate = endDate.AddDays(1);
                    outBuffer.Append(FormatDate(exclusiveEndDate));
                    break;
                case (Byte)'w':
                    // Wait/Pause not yet implemented
                    break;
                case (Byte)'i':
                    if (startColumn > length - 3)
                    {
                        errorMessage = "Incomplete operator at column " + startColumn + " in " + inString;
                        outBuffer.Append((char)inBuffer[startColumn]);
                        break;
                    }
                    outBuffer.Append(captures[inBuffer[(startColumn + 3)] % 16]);
                    skipCharCount = 3;
                    break;
                default:
                    errorMessage = "Illegal operator at column " + startColumn + " in " + inString;
                    outBuffer.Append((char)inBuffer[startColumn]);
                    break;
            }
            return skipCharCount;
        }

        private int ConvertFromBase64(string inBuffer, int startColumn, StringBuilder outBuffer)
        {
            int charsRead = 0;
            if(inBuffer[startColumn] == '\"')
            {
                outBuffer.Append("\"");
                ++charsRead;
            }
            StringBuilder base64Buffer = new StringBuilder();
            for(int index = 0; index < inBuffer.Length; ++index)
            {
                if(inBuffer[startColumn + index] == '\"')
                {
                    break;
                }
                base64Buffer.Append(inBuffer[startColumn + index]);
                ++charsRead;
            }
            string expanded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(base64Buffer.ToString()));
            outBuffer.Append(expanded);
            return charsRead;
        }

        private string FormatDate(DateTime date)
        {
            try
            {
                return String.Format(config.DateFormatsResponse, date);
            }
            catch(Exception ex)
            {
                errorMessage = "Bad Date Format " + config.DateFormatsResponse + " - " + ex.Message;
                return errorMessage;
            }
        }

    }
}
