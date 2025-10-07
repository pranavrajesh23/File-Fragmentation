using System;
using System.IO;

class Program
{
    static void Main()
    {
        string path = "IOFiles/Input.txt";

        if (!File.Exists(path))
        {
            File.Create(path).Close();
            Console.WriteLine("Enter text to append to the file (type 'END' on a new line to finish):");

            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                string input;
                while (true)
                {
                    input = Console.ReadLine();
                    if (input.ToUpper() == "END") break;
                    writer.WriteLine(input);
                }
            }
            Console.WriteLine($"Your text has been appended to {path}!");
        }
        else
        {
            Console.Write("Enter the number of characters per small file: ");
            if (!int.TryParse(Console.ReadLine(), out int chunkSize) || chunkSize <= 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive integer.");
                return;
            }

            string content = File.ReadAllText(path); 
            content=content.Replace("\r\n", "\n");
            int totalChars = content.Length;
            int fileCount = 1;
            int index = 0;
            int fileCountMax = (totalChars + chunkSize - 1) / chunkSize; // ceil division
            int padding = fileCountMax.ToString().Length; // number of digits to pad
            while (index < totalChars)
            {

                string smallFileName = Path.Combine("SplitFiles",fileCount.ToString().PadLeft(padding, '0') + ".txt");

                // Calculate how many characters to take
                int length = Math.Min(chunkSize, totalChars - index);
                string chunk = content.Substring(index, length);

                // Optional: convert \n back to Windows-style \r\n for readability
                chunk = chunk.Replace("\n", Environment.NewLine);

                File.WriteAllText(smallFileName, chunk);
                Console.WriteLine($"{smallFileName} created with {chunk.Length} visible characters.");

                index += length;
                fileCount++;
            }

            Console.WriteLine("Splitting completed!");
        }
    }
}
