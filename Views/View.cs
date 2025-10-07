using System;

namespace FileFragmentationConsole
{
    public class FileFragmentationView
    {
        public void ShowMessage(string msg)
        {
            Console.WriteLine(msg);
        }

        public string GetUserInput(string prompt = "")
        {
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}
