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
        }

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
}
