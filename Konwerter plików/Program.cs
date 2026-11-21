using System;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text;
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
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    i++;
                    continue;
                }

                if(line.StartsWith("#"))
                {
                    result.AppendLine(InterpretH(line));
                    i++;
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
                if (closing != null && input.Substring(index).StartsWith(closing))
                {
                    index += closing.Length;
                    return result.ToString();
                }

                if (input.Substring(index).StartsWith("**"))
                {
                    index += 2;

                    string content = ParseContent(input, ref index, "**");

                    result.Append("<strong>");
                    result.Append(content);
                    result.Append("</strong>");

                    continue;
                }

                if (input[index] == '*')
                {
                    index++;

                    string content = ParseContent(input, ref index, "*");

                    result.Append("<em>");
                    result.Append(content);
                    result.Append("</em>");

                    continue;
                }

                result.Append(input[index]);
                index++;
            }

            return result.ToString();
        }

    }
}