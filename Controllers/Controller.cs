using System;
using System.Collections.Generic;
using System.IO;

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

        public void Run()
        {
            _model.CleanUpFiles();
            _view.DisplayMessages(_model);

            StartFileProcess();
        }

        public void StartFileProcess()
        {
            _model.CreateInputFile();
            _view.ShowMessage("Enter text to append to the file (type 'END' on a new line to finish):");
            
            var lines = new List<string>();
            while (true)
            {
                string input = _view.GetUserInput();
                if (input.ToUpper() == "END") break;
                lines.Add(input);
            }

             _model.AppendUserText(lines);
             _view.DisplayMessages(_model);

            Console.WriteLine("Fragmentation Process");
            //string chunkInput = _view.GetUserInput("Enter the number of characters per small file: ");
            //if (!int.TryParse(chunkInput, out int chunkSize) || chunkSize <= 0)
            //{
            //    _view.ShowMessage("Invalid chunk size. Exiting...");
            //    return;
            //}
            int chunkSize = 0;
            while (true)
            {
                string chunkInput = _view.GetUserInput("Enter the number of characters per small file (1–50): ");

                if (int.TryParse(chunkInput, out chunkSize) && chunkSize >= 1 && chunkSize <= 50)
                    break;

                _view.ShowMessage("? Invalid size! Please enter a number between 1 and 50.");
            }


            _model.SplitFile(chunkSize);
            _view.DisplayMessages(_model);

            PostSplitMenu();
        }

        private void PostSplitMenu()
        {
            while (true)
            {
                _view.ShowMessage("\nPost-split options:");
                _view.ShowMessage("1. View a specific split file");
                _view.ShowMessage("2. Delete a fragment");
                _view.ShowMessage("3. Defragment and Compare");
                _view.ShowMessage("4. Delete all and create new file");
                _view.ShowMessage("5. Exit");
                string choice = _view.GetUserInput("Choose an option: ");

                switch (choice)
                {
                    case "1":
                        string viewFile = _view.GetUserInput("Enter fragment filename: ");
                        string content = _model.ViewFragment(viewFile);
                        _view.ShowMessage($"\n--- {viewFile} ---\n{content}\n--- End ---");
                        break;

                    case "2":
                        string deleteFile = _view.GetUserInput("Enter fragment filename to delete: ");
                        _view.ShowMessage(_model.DeleteFragment(deleteFile));
                        break;

                    case "3":
                        _view.ShowMessage(_model.DefragmentFiles());
                        _view.ShowMessage(_model.CompareInputAndOutput());
                        break;

                    case "4":
                        _model.DeleteAllFilesAndReset();
                        _view.DisplayMessages(_model);
                        StartFileProcess(); // restart everything
                        return;

                    case "5":
                        _view.ShowMessage("Exiting...");
                        return;

                    default:
                        _view.ShowMessage("Invalid option. Try again.");
                        break;
                }
            }
        }
    }
}
