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

            for (int i = 2; i < list.Count; i++)
            {
                string[] cells = list[i].Trim().Trim('|').Split('|');

                result.AppendLine("<tr>");

                foreach (string cell in cells)
                {
                    int j = 0;
                    result.AppendLine($"<td>{InlineParser.ParseContent(cell.Trim(), ref j)}</td>");
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
                result.AppendLine($"<th>{header.Trim()}</th>");
            }
            result.AppendLine("</tr>");
        }
    }
}
