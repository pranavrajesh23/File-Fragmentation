using System;
using System.IO;
using System.Linq;

namespace FileFragmentationConsole
{
    public class FileFragmentationModel
    {
        public string FilePath { get; set; }
        public string SplitFolder => "SplitFiles";
        public string OutFolder => "OutFiles";

        public void CreateOrAppendFile(string fullPath)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                using (var writer = new StreamWriter(fullPath, append: true))
                {
                    Console.WriteLine("Enter text for the file (type 'END' to finish):");
                    while (true)
                    {
                        string input = Console.ReadLine();
                        if (input.ToUpper() == "END") break;
                        writer.WriteLine(input);
                    }
                }

                Console.WriteLine($"? File '{Path.GetFileName(fullPath)}' created/appended successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while creating/appending file: {ex.Message}");
            }
        }

        public void SplitFile(int chunkSize)
        {
            try
            {
                string content = File.ReadAllText(FilePath).Replace("\r\n", "\n");
                int totalChars = content.Length;
                int fileCount = 1;
                int index = 0;
                int fileCountMax = (totalChars + chunkSize - 1) / chunkSize;
                int padding = fileCountMax.ToString().Length;

                Directory.CreateDirectory(SplitFolder);

                while (index < totalChars)
                {
                    string smallFileName = Path.Combine(SplitFolder, fileCount.ToString().PadLeft(padding, '0') + ".txt");
                    int length = Math.Min(chunkSize, totalChars - index);
                    string chunk = content.Substring(index, length).Replace("\n", Environment.NewLine);

                    File.WriteAllText(smallFileName, chunk);
                    index += length;
                    fileCount++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while splitting file: {ex.Message}");
            }
        }

        public void ViewFragment()
        {
            try
            {
                var fragments = Directory.GetFiles(SplitFolder, "*.txt").OrderBy(f => f).ToArray();
                if (fragments.Length == 0)
                {
                    Console.WriteLine("No fragments found!");
                    return;
                }

                for (int i = 0; i < fragments.Length; i++)
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(fragments[i])}");

                Console.Write("Enter fragment number: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= fragments.Length)
                    Console.WriteLine(File.ReadAllText(fragments[index - 1]));
                else
                    Console.WriteLine("Invalid choice!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while viewing fragment: {ex.Message}");
            }
        }

        public void DeleteFragment()
        {
            try
            {
                var fragments = Directory.GetFiles(SplitFolder, "*.txt").OrderBy(f => f).ToArray();
                if (fragments.Length == 0)
                {
                    Console.WriteLine("No fragments to delete!");
                    return;
                }

                for (int i = 0; i < fragments.Length; i++)
                    Console.WriteLine($"{i + 1}. {Path.GetFileName(fragments[i])}");

                Console.Write("Enter fragment number to delete: ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= fragments.Length)
                {
                    File.Delete(fragments[index - 1]);
                    Console.WriteLine("?? Fragment deleted successfully!");
                }
                else
                    Console.WriteLine("Invalid choice!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while deleting fragment: {ex.Message}");
            }
        }

        public void DefragmentFiles()
        {
            try
            {
                var fragments = Directory.GetFiles(SplitFolder, "*.txt").OrderBy(f => f).ToArray();
                if (fragments.Length == 0)
                {
                    Console.WriteLine("No fragments found!");
                    return;
                }

                string combined = string.Join("", fragments.Select(f => File.ReadAllText(f)));
                string outputPath = Path.Combine(OutFolder, "output.txt");
                File.WriteAllText(outputPath, combined);
                Console.WriteLine("? Defragmented successfully into 'output.txt'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while defragmenting: {ex.Message}");
            }
        }

        public void DeleteAllFragments()
        {
            try
            {
                if (Directory.Exists(SplitFolder))
                {
                    foreach (var file in Directory.GetFiles(SplitFolder))
                        File.Delete(file);
                   
                }
                if (Directory.Exists(OutFolder))
                {
                    foreach (var file in Directory.GetFiles(SplitFolder))
                        File.Delete(file);
                    
                }
                if (Directory.Exists("IOFiles"))
                {
                    foreach (var file in Directory.GetFiles(SplitFolder))
                        File.Delete(file);
                    
                }
                Console.WriteLine("?? All fragments deleted!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"? Error while deleting fragments: {ex.Message}");
            }
        }
    }
}
