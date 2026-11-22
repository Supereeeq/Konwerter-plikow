using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Konwerter_plików
{
    internal class ListParser
    {
        static bool IsOrderedListItem(string line)
        {
            return Regex.IsMatch(line.Trim(), @"^\d+\.\s+");
        }

        static int GetIndentation(string line)
        {
            int count = 0;

            foreach (char c in line)
            {
                if (c == ' ')
                    count++;
                else if (c == '\t')
                    count += 4;
                else
                    break;
            }

            return count;
        }

        public static string InterpretUnorderedList(List<string> lines)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<ul>");

            int i = 0;
            while (i < lines.Count)
            {
                string line = lines[i];
                int indentation = GetIndentation(line);

                string content = Regex.Replace(line.Trim(), @"^[+\-*]", "");

                result.Append("<li>" + BlockParser.InterpretBlocks(content));

                i++;

                List<string> nestedLines = new List<string>();
                while (i < lines.Count &&
                       GetIndentation(lines[i]) > indentation)
                {
                    nestedLines.Add(lines[i]);
                    i++;
                }

                if (nestedLines.Count > 0)
                {
                    if (IsOrderedListItem(nestedLines[0]))
                    {
                        result.Append(InterpretOrderedList(nestedLines));
                    }
                    else
                    {
                        result.Append(InterpretUnorderedList(nestedLines));
                    }
                }

                result.AppendLine("</li>");
            }

            result.AppendLine("</ul>");
            return result.ToString();
        }

        public static string InterpretOrderedList(List<string> lines)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<ol>");

            int i = 0;

            while (i < lines.Count)
            {
                string line = lines[i];
                int indentation = GetIndentation(line);

                string content = Regex.Replace(line.Trim(), @"^\d+\.\s+", "");

                result.Append("<li>" + BlockParser.InterpretBlocks(content));

                i++;

                List<string> nestedLines = new List<string>();

                while (i < lines.Count &&
                       GetIndentation(lines[i]) > indentation)
                {
                    nestedLines.Add(lines[i]);
                    i++;
                }

                if (nestedLines.Count > 0)
                {
                    if (IsOrderedListItem(nestedLines[0]))
                    {
                        result.Append(InterpretOrderedList(nestedLines));
                    }
                    else
                    {
                        result.Append(InterpretUnorderedList(nestedLines));
                    }
                }

                result.AppendLine("</li>");
            }

            result.AppendLine("</ol>");
            return result.ToString();
        }
    }
}
