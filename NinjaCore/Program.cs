using System;
using System.IO;
using System.Linq;

namespace NinjaCore
{
    class Program
    {
        private static string filename;
        private static string newStartRange;
        private static string newEndRange;
        private static int startNr;
        private static int endNr;

        static void Main(string[] args)
        {
            Console.WriteLine("NinjaCore -path \"path to app.json\" -startRange 50000 -endRange 50050");
            if(args.Length > 0)
            {
                for(var i = 0; i < args.Length; i++)
                {
                    if(args[i] == "-path")
                    {
                        filename = args[i + 1];

                        if (!File.Exists(filename))
                        {
                            throw new Exception("Invalid filename!");
                        }
                    }
                    if (args[i] == "-startRange")
                    {
                        newStartRange = args[i + 1];
                        if (!Int32.TryParse(newStartRange, out startNr))
                        {
                            throw new Exception("Start range is invalid");
                        }
                    }
                    if (args[i] == "-endRange")
                    {
                        newEndRange = args[i + 1];
                        if (!Int32.TryParse(newEndRange, out endNr))
                        {
                            throw new Exception("End range is invalid");
                        }
                    }
                }
                Console.WriteLine("NinjaCore -path " + filename + " -startRange " + newStartRange + " -endRange " + newEndRange);
                new Renamer().RenameFiles(filename, startNr, endNr);
            }
            else
            {
                Console.WriteLine("Please enter correct parameters!");
            }

        }
    }
}
