using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spar
{
    public static class Util
    {
        public static string GetCardData(string cardMessage)
        {
            return getAlphaNumeric(RemoveUnicode(getSubString(cardMessage, "<Data>", "</Data>")));
        }
        private static string RemoveUnicode(string text)
        {
            var sb = new StringBuilder();
            try
            {
                foreach (char c in text)
                {
                    int charCode = (int)c;
                    if (charCode > 13 || c == '\t' || c == '\n')
                        sb.Append(c);

                }
                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return text;
        }
        private static string getAlphaNumeric(string text)
        {
            var sb = new StringBuilder();
            try
            {
                foreach (char c in text)
                {
                    if (isAlphaNumeric(c))
                        sb.Append(c);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return sb.ToString();
        }
        private static bool isAlphaNumeric(char code)
        {
            if (('A' <= code && code <= 'Z') || ('a' <= code && code <= 'z'))
                return true;
            if (('0' <= code && code <= '9'))
                return true;
            return false;
        }
        public static string getSubString(string str, string startStr, string endStr)
        {
            try
            {
                str = str.Trim();
                int iPos1 = str.ToLower().IndexOf(startStr.ToLower().Trim()); //<Device>
                int iPos2 = str.ToLower().IndexOf(endStr.ToLower().Trim(), iPos1 + 1); //</Device>
                if (iPos1 != -1 && iPos2 != -1 && (iPos2 > iPos1))
                {
                    string s = str.Substring(iPos1 + startStr.Length, (iPos2 - (iPos1 + startStr.Length)));
                    return s;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return "";
        }
    }
}
