using System;

namespace FileFragmentationConsole
{
    public class FileFragmentationView
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        public string GetUserInput(string prompt = "")
        {
            if (!string.IsNullOrEmpty(prompt))
                Console.Write(prompt);
            return Console.ReadLine();
        }

        public void DisplayMessages(FileFragmentationModel model)
        {
            foreach (var msg in model.Messages)
            {
                Console.WriteLine(msg);
            }
            model.Messages.Clear();
        }
    }
}
