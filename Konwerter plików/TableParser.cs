using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konwerter_plików
{
    internal class TableParser
    {
        public static string InterpretTable(List<string> list)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("<table>");

            InterpretHeaders(list[0], result);

            string[] alignments = InterpretAlignment(list[1]);
            
            

            for (int i = 2; i < list.Count; i++)
            {
                string[] cells = list[i].Trim().Trim('|').Split('|');

                result.AppendLine("<tr>");

                for (int j = 0; j < cells.Length; j++)
                {
                    int k = 0;
                    result.AppendLine($"<td style=\"text-align: {alignments[j]}\">" +
                        $"{InlineParser.ParseContent(cells[j].Trim(), ref k)}</td>");
                }

                result.AppendLine("</tr>");
            }

            result.AppendLine("</table>");

            return result.ToString();
        }

        private static void InterpretHeaders(string headerLine, StringBuilder result)
        {
            result.AppendLine("<tr>");
            string[] headers = headerLine.Trim().Trim('|').Split('|');
            foreach (string header in headers)
            {
                int k = 0;
                result.AppendLine($"<th>{InlineParser.ParseContent(header.Trim(), ref k)}</th>");
            }
            result.AppendLine("</tr>");
        }

        private static string[] InterpretAlignment(string alignmentLine)
        {
            List<string> alignmentList = new List<string>();
            string[] alignments = alignmentLine.Trim().Trim('|').Split('|');

            foreach(string alignment in alignments)
            {
                string value = alignment.Trim();

                if (value.StartsWith(":") && value.EndsWith(":"))
                {
                    alignmentList.Add("center");
                }
                else if (value.EndsWith(":"))
                {
                    alignmentList.Add("right");
                }
                else
                {
                    alignmentList.Add("left");
                }
            }

            return alignmentList.ToArray();
        }
    }
}
