using System;
using System.IO;
using System.Linq;

namespace FileFragmentationConsole
{
    public class FileFragmentationController
    {
        private readonly FileFragmentationModel _model;
        private readonly FileFragmentationView _view;

        public FileFragmentationController(FileFragmentationModel model, FileFragmentationView view)
        {
            _model = model;
            _view = view;
        }

        public void StartApp()
        {
            try
            {
                Directory.CreateDirectory("IOFiles");
                Directory.CreateDirectory("SplitFiles");
                Directory.CreateDirectory("OutFiles");
                foreach (var file in Directory.GetFiles("SplitFiles"))
                    File.Delete(file);
                foreach (var file in Directory.GetFiles("IOFiles"))
                    File.Delete(file);
                foreach (var file in Directory.GetFiles("OutFiles"))
                    File.Delete(file);

                while (true)
                {
                    _view.ShowMessage("\n=== FILE FRAGMENTATION SYSTEM ===");
                    _view.ShowMessage("1. Create or append a file");
                    _view.ShowMessage("2. Fragment an existing file");
                    _view.ShowMessage("3. Exit");

                    string choice = _view.GetUserInput("Enter your choice: ");

                    switch (choice)
                    {
                        case "1":
                            CreateOrAppendFile();
                            break;
                        case "2":
                            FragmentExistingFile();
                            break;
                        case "3":
                            _view.ShowMessage("Exiting program... Goodbye!");
                            return;
                        default:
                            _view.ShowMessage("Invalid choice, try again!");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"? Unexpected error: {ex.Message}");
            }
        }

        private void CreateOrAppendFile()
        {
            try
            {
                string fileName = _view.GetUserInput("Enter file name (e.g., MyText.txt): ");
                string fullPath = Path.Combine("IOFiles", fileName);
                _model.CreateOrAppendFile(fullPath);
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"? Error in CreateOrAppendFile: {ex.Message}");
            }
        }

        private void FragmentExistingFile()
        {
            try
            {
                var files = Directory.GetFiles("IOFiles", "*.txt");
                if (files.Length == 0)
                {
                    _view.ShowMessage("No files found. Please create one first!");
                    return;
                }

                _view.ShowMessage("\nAvailable files:");
                for (int i = 0; i < files.Length; i++)
                    _view.ShowMessage($"{i + 1}. {Path.GetFileName(files[i])}");

                string choice = _view.GetUserInput("Select file number: ");
                if (!int.TryParse(choice, out int index) || index < 1 || index > files.Length)
                {
                    _view.ShowMessage("Invalid selection!");
                    return;
                }

                _model.FilePath = files[index - 1];
                StartFileFragmentation();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"? Error while fragmenting: {ex.Message}");
            }
        }

        private void StartFileFragmentation()
        {
            try
            {
                int chunkSize = 0;
                while (true)
                {
                    string chunkInput = _view.GetUserInput("Enter number of characters per fragment (1–50): ");
                    if (int.TryParse(chunkInput, out chunkSize) && chunkSize >= 1 && chunkSize <= 50)
                        break;
                    _view.ShowMessage("? Invalid input! Please enter a number between 1 and 50.");
                }

                _model.SplitFile(chunkSize);
                _view.ShowMessage("? File successfully fragmented!");

                PostSplitMenu();
            }
            catch (Exception ex)
            {
                _view.ShowMessage($"? Error in StartFileFragmentation: {ex.Message}");
            }
        }

        private void PostSplitMenu()
        {
            while (true)
            {
                try
                {
                    _view.ShowMessage("\n--- Post-Split Menu ---");
                    _view.ShowMessage("1. View a specific fragment");
                    _view.ShowMessage("2. Delete a fragment");
                    _view.ShowMessage("3. Defragment all fragments");
                    _view.ShowMessage("4. Delete all fragments & return to main menu");
                    _view.ShowMessage("5. Exit to main menu");

                    string choice = _view.GetUserInput("Enter your choice: ");
                    switch (choice)
                    {
                        case "1": _model.ViewFragment(); break;
                        case "2": _model.DeleteFragment(); break;
                        case "3": _model.DefragmentFiles(); break;
                        case "4": _model.DeleteAllFragments(); return;
                        case "5": return;
                        default: _view.ShowMessage("Invalid option! Try again."); break;
                    }
                }
                catch (Exception ex)
                {
                    _view.ShowMessage($"? Error in PostSplitMenu: {ex.Message}");
                }
            }
        }
    }
}
