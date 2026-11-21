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

            string[] plikMd = File.ReadAllLines(localization);
            foreach(string line in plikMd)
            {
                Console.WriteLine(InterpretLine(line));
                //Console.WriteLine(line);
            }

        }

        static string InterpretLine(string line)
        {
            if (line == null || line == "") return "";

            line = line.Trim();
            if (line[0] == '#')
            {
                return InterpretH(line);
            }

            return InterpretP(line);
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
            if(line.Contains("*"))
            {
                int index = 0;
                return ("<p>" + ParseContent(line, ref index) + "</p>");
            }
            return line;
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