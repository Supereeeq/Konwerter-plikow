using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace konwerterPlikow
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine();
            readFile("../../../../test.txt");
        }

        static void readFile(string localization)
        {
            if(!File.Exists(localization))
            {
                Console.WriteLine("niema");
                return;
                
            }

            string mdFile = File.ReadAllText(localization);

            string html = InterpretBlocks(mdFile);
            
            Console.WriteLine(html);

        }

        static string InterpretBlocks(string mdFile)
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

                if(line.Trim().StartsWith("#"))
                {
                    result.AppendLine(InterpretH(line.Trim()));
                    i++;
                }
                else if(line.Trim().StartsWith(">"))
                {
                    List<string> codeLines = new List<string>();

                    while(i < lines.Length && lines[i].Trim().StartsWith(">"))
                    {
                        codeLines.Add(lines[i].Trim().Substring(1).Trim());
                        i++;
                    }
                    result.AppendLine($"<blockquote>{InterpretBlocks(string.Join("\n", codeLines))}</blockquote>");
                }
                else if(Regex.IsMatch(line.Trim(), @"^\d+\.\s+"))
                {
                    List<string> orderedListLines = new List<string>();
                    Regex regex = new(@"^\d+\.\s+");

                    while (i < lines.Length && Regex.IsMatch(lines[i].Trim(), regex.ToString()))
                    {
                        orderedListLines.Add(lines[i]);
                        i++;
                    }
                    result.AppendLine(InterpretOrderedList(orderedListLines));
                }
                else if(line.Trim().StartsWith("-") || line.Trim().StartsWith("+") || line.Trim().StartsWith("*"))
                {
                    List<string> unorderedListLines = new List<string>();

                    while (i < lines.Length)
                    {
                        unorderedListLines.Add(lines[i]);
                        i++;
                    }
                    result.AppendLine(InterpretUnorderedList(unorderedListLines));

                }
                else
                {
                    List<string> paragraphLines = new List<string>();

                    while (i < lines.Length && !IsBlockStart(lines[i]) && !string.IsNullOrWhiteSpace(lines[i]))
                    {
                        paragraphLines.Add(lines[i].Trim());
                        i++;
                    }

                    result.AppendLine(InterpretP(string.Join("\n", paragraphLines)));
                }
            }

            return result.ToString();
        }

        static bool IsBlockStart(string line)
        {
            return line.StartsWith("#") ||
                   line.StartsWith(">");
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

        static string InterpretUnorderedList(List<string> lines)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<ul>");

            int i = 0;
            while (i < lines.Count)
            {
                string line = lines[i];
                int indentation = GetIndentation(line);

                string content = Regex.Replace(line.Trim(), @"^[+\-*]", "");

                result.Append("<li>" + InterpretBlocks(content));

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
                    result.Append(InterpretUnorderedList(nestedLines));
                }

                result.AppendLine("</li>");
            }

            result.AppendLine("</ol>");
            return result.ToString();
        }

        static string InterpretOrderedList(List<string> lines)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine("<ol>");

            int i = 0;

            while (i < lines.Count)
            {
                string line = lines[i];
                int indentation = GetIndentation(line);

                string content = Regex.Replace(line.Trim(), @"^\d+\.\s+", "");

                result.Append("<li>" + InterpretBlocks(content));

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
                    result.Append(InterpretOrderedList(nestedLines));
                }

                result.AppendLine("</li>");
            }

            result.AppendLine("</ol>");
            return result.ToString();
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
            return ("<p>" + ParseContent(line, ref index) + "</p>");
        }

        private static string ParseContent(string input, ref int index, string closing = null)
        {
            var result = new StringBuilder();

            while (index < input.Length)
            {
                if (input[index] == '\\' && index + 1 < input.Length)
                {
                    result.Append(System.Net.WebUtility.HtmlEncode(
                        input[index + 1].ToString()
                    ));

                    index += 2;
                    continue;
                }

                int stars = CountStars(input, index);

                if (closing != null && CanClose(closing, stars))
                {
                    index += closing.Length;
                    return result.ToString();
                }

                if (stars >= 2 && HasClosingDelimiter(input, index + 2, "**"))
                {
                    index += 2;

                    string content = ParseContent(input, ref index, "**");

                    result.Append("<strong>");
                    result.Append(content);
                    result.Append("</strong>");

                    continue;
                }

                if (stars == 1 && HasClosingDelimiter(input, index + 1, "*"))
                {
                    index++;

                    string content = ParseContent(input, ref index, "*");

                    result.Append("<em>");
                    result.Append(content);
                    result.Append("</em>");

                    continue;
                }

                result.Append(System.Net.WebUtility.HtmlEncode(
                    input[index].ToString()
                ));

                index++;
            }

            return result.ToString();
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