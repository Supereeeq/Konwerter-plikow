using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Konwerter_plików
{
    internal class InlineParser
    {
        public static string ParseContent(string input, ref int index, string closing = null)
        {
            var result = new StringBuilder();

            while (index < input.Length)
            {
                if (TryInterpretEscape(input, ref index, result))
                    continue;

                if (TryInterpretCode(input, ref index, result))
                    continue;

                int stars = CountStars(input, index);

                if (closing != null && CanClose(closing, stars))
                {
                    index += closing.Length;
                    return result.ToString();
                }

                if (TryInterpretEmphasis(input, ref index, result))
                    continue;

                if (TryInterpretImage(input, ref index, result))
                    continue;

                result.Append(System.Net.WebUtility.HtmlEncode(
                    input[index].ToString()
                ));

                index++;
            }

            return result.ToString();
        }

        private static bool TryInterpretCode(string input, ref int index, StringBuilder result)
        {
            if (!input.Substring(index).StartsWith("`"))
                return false;

            int end = input.IndexOf("`", index + 1);

            string content = input.Substring(index + 1, end - index - 1);

            result.Append($"<code>{content}</code>");

            index = end + 1;

            return true;
        }

        private static bool TryInterpretEscape(string input, ref int index, StringBuilder result)
        {
            if (input[index] != '\\' || index + 1 >= input.Length)
                return false;

            result.Append(System.Net.WebUtility.HtmlEncode(
                input[index + 1].ToString()
            ));

            index += 2;

            return true;
        }

        private static bool TryInterpretEmphasis(string input, ref int index, StringBuilder result)
        {
            int stars = CountStars(input, index);

            if (stars >= 2 && HasClosingDelimiter(input, index + 2, "**"))
            {
                index += 2;

                string content = ParseContent(input, ref index, "**");

                result.Append("<strong>");
                result.Append(content);
                result.Append("</strong>");

                return true;
            }

            if (stars == 1 && HasClosingDelimiter(input, index + 1, "*"))
            {
                index++;

                string content = ParseContent(input, ref index, "*");

                result.Append("<em>");
                result.Append(content);
                result.Append("</em>");

                return true;
            }

            return false;
        }

        private static bool TryInterpretImage(string input, ref int index, StringBuilder result)
        {
            if (!input.Substring(index).StartsWith("!["))
                return false;

            string image = InterpretImage(input, ref index);

            result.Append(image);

            return true;
        }

        private static string InterpretImage(string input, ref int index)
        {
            int altStart = index + 2;
            int altEnd = input.IndexOf("]", altStart);

            if (altEnd == -1)
                return null;

            int srcStart = input.IndexOf("(", altEnd) + 1;

            if (srcStart == 0)
                return null;

            int srcEnd = input.IndexOf(")", srcStart);

            if (srcEnd == -1)
                return null;

            string alt = input.Substring(altStart, altEnd - altStart);
            string src = input.Substring(srcStart, srcEnd - srcStart);

            index = srcEnd + 1;

            alt = System.Net.WebUtility.HtmlEncode(alt);
            src = System.Net.WebUtility.HtmlEncode(src);

            return $"<img alt=\"{alt}\" src=\"{src}\" />";
        }

        private static int CountStars(string input, int index)
        {
            int count = 0;

            while (index + count < input.Length &&
                   input[index + count] == '*')
            {
                count++;
            }

            return count;
        }

        private static bool CanClose(string closing, int stars)
        {
            if (closing == "**")
                return stars >= 2;

            if (closing == "*")
                return stars % 2 == 1;

            return false;
        }

        private static bool HasClosingDelimiter(string input, int startIndex, string delimiter)
        {
            for (int i = startIndex; i < input.Length; i++)
            {
                if (input[i] == '\\')
                {
                    i++;
                    continue;
                }

                if (input[i] != '*')
                    continue;

                int stars = CountStars(input, i);

                if (delimiter == "**" && stars >= 2)
                    return true;

                if (delimiter == "*" && stars % 2 == 1)
                    return true;

                i += stars - 1;
            }

            return false;
        }

    }
}
