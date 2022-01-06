using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NinjaCore
{
    public class Renamer
    {
        private Dictionary<string, string> dictRange = new Dictionary<string, string>();
        private string currentOldKey;
        private string currentNewKey;
        private int incidents = 0;
        private string dir;
        private int oldStartRange = 0;
        private int oldEndRange = 0;
        private string filename = "";
        private int newStartRange;
        private int newEndRange;

        private void RenameAllIdsInFiles()
        {
            var directories = Directory.GetDirectories(dir);

            foreach (var pair in dictRange)
            {
                currentOldKey = pair.Key;
                currentNewKey = pair.Value;
                ReadDirectories(directories);
            }

            var newString = "";
            using (var streamR = new StreamReader(filename))
            {

                var text = streamR.ReadToEnd();
                newString = text;
                newString = text.Replace(oldStartRange.ToString(), newStartRange.ToString());
                newString = newString.Replace(oldEndRange.ToString(), newEndRange.ToString());
            }
            using (var streamW = new StreamWriter(filename))
            {
                if (newString.Length > 0)
                {
                    streamW.Write(newString);
                }
            }
        }

        private void ReadDirectories(string[] directories)
        {
            foreach (var dir in directories)
            {
                var files = Directory.GetFiles(dir);
                foreach (var file in files)
                {
                    SearchAndReplaceInFile(file);

                }
                ReadDirectories(Directory.GetDirectories(dir));
            }
        }

        private void SearchAndReplaceInFile(string file)
        {
            var oldText = "";
            var newString = "";
            bool editFile = false;
            using (var streamR = new StreamReader(file))
            {

                oldText = streamR.ReadToEnd();
                //newString = oldText;
                if (oldText.Contains(currentOldKey))
                {
                    newString = oldText.Replace(currentOldKey, currentNewKey);
                    editFile = true;
                }
            }
            if (editFile)
            {
                using (var streamW = new StreamWriter(file))
                {
                    if (!newString.Equals(oldText))
                    {
                        streamW.Write(newString);
                        incidents += 1;
                    }
                }
            }
        }

        public void RenameFiles(string filePath, int startRange, int endRange)
        {
            filename = filePath;
            ReadAppFile();
            var oldStart = oldStartRange;
            var oldEnd = oldEndRange;
            newStartRange = startRange;
            newEndRange = endRange;
            for (var i = oldStart; i < oldEnd + 1; i++)
            {
                dictRange.Add(i.ToString(), newStartRange.ToString());
                newStartRange += 1;
            }

            RenameAllIdsInFiles();

            Console.WriteLine("changed " + incidents + " id's");
            Console.WriteLine("including app.json");
            Console.WriteLine("Remember to generate new permissionset in AL");
        }

        private void ReadAppFile()
        {
            if (!string.IsNullOrWhiteSpace(filename) && filename.EndsWith("app.json"))
            {
                dir = Path.GetDirectoryName(filename);
                dir = dir + "\\src";

                using (var streamR = new StreamReader(filename))
                {
                    while (!streamR.EndOfStream)
                    {
                        var json = streamR.ReadLine();
                        if (json.Contains("\"from\":"))
                        {
                            string fromId = json.Trim().Split(':').Last().Replace(',', ' ').Trim();
                            oldStartRange = Int32.Parse(fromId);
                            Console.WriteLine("Old start range: " + oldStartRange.ToString());
                        }
                        if (json.Contains("\"to\":"))
                        {
                            string toId = json.Trim().Split(':').Last();
                            oldEndRange = Int32.Parse(toId);
                            Console.WriteLine("Old end range: " + oldEndRange.ToString());
                        }
                    }

                }
            }
        }
    }
}
