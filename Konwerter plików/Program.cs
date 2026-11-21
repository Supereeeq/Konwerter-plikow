using System.IO;

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
               
                int start = line.IndexOf("**");

                while (start > -1)
                {
                    int end = line.IndexOf("**", start + 2);

                    line = line.Substring(0, start) + "<strong>" + line.Substring(start + 2, end - start - 2) + "</strong>" + line.Substring(end + 2);

                    start = line.IndexOf("**", end);
                }
            }

            return line;
        }


    }
}