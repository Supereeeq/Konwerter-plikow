using Konwerter_plików;
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

            string html = BlockParser.InterpretBlocks(mdFile);
            
            writeFile(html);

        }

        static void writeFile(string content)
        {
            //File.Create("../../../../test.html");
            File.WriteAllText("../../../../test.html", content);
            Console.WriteLine("pomyślnie zapisano");
        }

        
    }
}