using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileFragmentationConsole
{
    public class FileFragmentationModel
    {
        public string FilePath { get; set; } = "IOFiles/Input.txt";
        public int ChunkSize { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public string SplitFolder { get; set; } = "SplitFiles";
        public string OutputFile { get; set; } = "IOFiles/Output.txt";

        public void CleanUpFiles()
        {
            Directory.CreateDirectory(SplitFolder);
            foreach (var file in Directory.GetFiles(SplitFolder, "*.txt"))
            {
                File.Delete(file);
            }

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            Messages.Add("Clean startup: All previous text files deleted.");
        }

        public void CreateInputFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath) ?? "");
            File.Create(FilePath).Close();
            Messages.Add($"New file created at {FilePath}");
        }

        public void AppendUserText(IEnumerable<string> lines)
        {
            using (StreamWriter writer = new StreamWriter(FilePath, append: true))
            {
                foreach (var line in lines)
                {
                    if (line.ToUpper() == "END") break;
                    writer.WriteLine(line);
                }
            }
            Messages.Add($"Text appended to {FilePath}");
        }

        public void SplitFile(int chunkSize)
        {
            Directory.CreateDirectory(SplitFolder);
            string content = File.ReadAllText(FilePath).Replace("\r\n", "\n");

            int totalChars = content.Length;
            int fileCount = 1;
            int index = 0;
            int fileCountMax = (totalChars + chunkSize - 1) / chunkSize;
            int padding = fileCountMax.ToString().Length;

            while (index < totalChars)
            {
                string smallFileName = Path.Combine(SplitFolder, fileCount.ToString().PadLeft(padding, '0') + ".txt");
                int length = Math.Min(chunkSize, totalChars - index);
                string chunk = content.Substring(index, length).Replace("\n", Environment.NewLine);

                File.WriteAllText(smallFileName, chunk);
                Messages.Add($"{smallFileName} created with {chunk.Length} characters.");

                index += length;
                fileCount++;
            }

            Messages.Add("Splitting completed successfully!");
        }

        public string ViewFragment(string filename)
        {
            string fullPath = Path.Combine(SplitFolder, filename);
            if (!File.Exists(fullPath))
                return "File does not exist.";

            return File.ReadAllText(fullPath);
        }

        public string DeleteFragment(string filename)
        {
            string fullPath = Path.Combine(SplitFolder, filename);
            if (!File.Exists(fullPath))
                return "File not found.";

            File.Delete(fullPath);
            return $"{filename} deleted successfully.";
        }

        public string DefragmentFiles()
        {
            Directory.CreateDirectory(SplitFolder);
            var fragmentFiles = Directory.GetFiles(SplitFolder)
                .Where(f => Path.GetFileName(f) != "output.txt")
                .OrderBy(f => f)
                .ToArray();

            using (StreamWriter writer = new StreamWriter(OutputFile, false))
            {
                foreach (var file in fragmentFiles)
                {
                    writer.Write(File.ReadAllText(file));
                }
            }
            return $"All fragments merged into {OutputFile}";
        }

        public string CompareInputAndOutput()
        {
            if (!File.Exists(FilePath))
                return "Input file not found.";

            if (!File.Exists(OutputFile))
                return "Output file not found.";

            string inputContent = File.ReadAllText(FilePath).Replace("\r\n", "\n");
            string outputContent = File.ReadAllText(OutputFile).Replace("\r\n", "\n");

            if (inputContent == outputContent)
                return "Files are the SAME!";
            else
                return "Files are DIFFERENT!";
        }

        public void DeleteAllFilesAndReset()
        {
            if (Directory.Exists(SplitFolder))
            {
                foreach (var file in Directory.GetFiles(SplitFolder, "*.txt"))
                    File.Delete(file);
            }
            if (Directory.Exists("IOFiles"))
            {
                foreach (var file in Directory.GetFiles("IOFiles", "*.txt"))
                    File.Delete(file);
            }
            Messages.Add("All fragments and input file deleted. Ready to create a new file.");
        }
    }
}
