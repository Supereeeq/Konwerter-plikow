using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Konwerter_plików
{
    public static class BlockParser
    {
        public static string InterpretBlocks(string mdFile)
        {
            string[] lines = mdFile.Split('\n');

            StringBuilder result = new StringBuilder();

            int i = 0;

            while (i < lines.Length)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line.Trim()))
                {
                    i++;
                    continue;
                }

                if (line.Trim().StartsWith("#"))
                {
                    result.AppendLine("");
                    result.AppendLine(InterpretH(line.Trim()));
                    i++;
                }
                else if (IsHorizontalRule(line))
                {
                    result.AppendLine(InterpretHorizontalRule());
                    i++;
                    continue;
                }
                else if (line.Trim().StartsWith(">"))
                {
                    List<string> codeLines = new List<string>();

                    while (i < lines.Length && lines[i].Trim().StartsWith(">"))
                    {
                        codeLines.Add(lines[i].Trim().Substring(1).Trim());
                        i++;
                    }
                    result.AppendLine($"<blockquote>{InterpretBlocks(string.Join("\n", codeLines))}</blockquote>");
                }
                else if (Regex.IsMatch(line.Trim(), @"^\d+\.\s+"))
                {
                    List<string> orderedListLines = new List<string>();
                    Regex regex = new(@"^\d+\.\s+");

                    while (i < lines.Length && Regex.IsMatch(lines[i].Trim(), regex.ToString()))
                    {
                        orderedListLines.Add(lines[i]);
                        i++;
                    }
                    result.AppendLine(ListParser.InterpretOrderedList(orderedListLines));
                }
                else if (line.Trim().StartsWith("-") || line.Trim().StartsWith("+") || line.Trim().StartsWith("*"))
                {
                    List<string> unorderedListLines = new List<string>();

                    while (i < lines.Length)
                    {
                        unorderedListLines.Add(lines[i]);
                        i++;
                    }
                    result.AppendLine(ListParser.InterpretUnorderedList(unorderedListLines));

                }
                else if(line.Contains('|') && IsTableSeperator(lines[i + 1]))
                {
                    List<string> tableListLines = new List<string>();

                    while(i < lines.Length && lines[i].Contains('|'))
                    {
                        tableListLines.Add(lines[i]);
                        i++;
                    }
                    result.AppendLine(TableParser.InterpretTable(tableListLines));
                }
                else
                {
                    List<string> paragraphLines = new List<string>();

                    while (i < lines.Length && !IsBlockStart(lines[i]) && !string.IsNullOrWhiteSpace(lines[i]))
                    {
                        paragraphLines.Add(lines[i].Trim());
                        i++;
                    }

                    result.Append(InterpretP(string.Join("\n", paragraphLines)));
                }
            }

            return result.ToString();
        }

        private static bool IsTableSeperator(string line)
        {
            string[] cells = line.Trim().Trim('|').Split('|');

            foreach (string cell in cells)
            {
                string value = cell.Trim();

                if (!Regex.IsMatch(value, @"^:?-{3,}:?$"))
                    return false;
            }

            return true;
        }

        private static bool IsBlockStart(string line)
        {
            return line.StartsWith("#") ||
                   line.StartsWith(">");
        }

        private static bool IsHorizontalRule(string line)
        {
            string trimmed = line.Trim();

            return Regex.IsMatch(trimmed, @"^([-*_])(?:\s*\1){2,}$");
        }

        private static string InterpretHorizontalRule()
        {
            return "<hr />";
        }

        static string InterpretH(string line)
        {
            if (line[0] == '#' && line[1] == '#' && line[2] == '#' && line[3] == '#' && line[4] == '#' && line[5] == '#')
            {
                return ($"<h6>{line.Substring(6).Trim()}</h6>");
            }
            else if (line[0] == '#' && line[1] == '#' && line[2] == '#' && line[3] == '#' && line[4] == '#')
            {
                return ($"<h5>{line.Substring(5).Trim()}</h5>");
            }
            else if (line[0] == '#' && line[1] == '#' && line[2] == '#' && line[3] == '#')
            {
                return ($"<h4>{line.Substring(4).Trim()}</h4>");
            }
            else if (line[0] == '#' && line[1] == '#' && line[2] == '#')
            {
                return ($"<h3>{line.Substring(3).Trim()}</h3>");
            }
            else if (line[0] == '#' && line[1] == '#')
            {
                return ($"<h2>{line.Substring(2).Trim()}</h2>");
            }
            else if (line[0] == '#')
            {
                return ($"<h1>{line.Substring(1).Trim()}</h1>");
            }
            throw new Exception($"{line} nie zawieta elementu <hx>");
        }

        static string InterpretP(string line)
        {
            int index = 0;
            return ("<p>" + InlineParser.ParseContent(line, ref index) + "</p>");
        }

    }
}
