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

        public void Run()
        {
            Directory.CreateDirectory("IOFiles");
            Directory.CreateDirectory(_model.SplitFolder);

            if (!File.Exists(_model.FilePath))
            {
                //Directory.CreateDirectory(Path.GetDirectoryName(_model.FilePath) ?? string.Empty);
                //File.Create(_model.FilePath).Close();
                File.Create(_model.FilePath).Close();
                _view.ShowMessage("Enter text to append to the file (type 'END' on a new line to finish):");

                using (StreamWriter writer = new StreamWriter(_model.FilePath, append: true))
                {
                    while (true)
                    {
                        string input = _view.GetUserInput();
                        if (input.ToUpper() == "END") break;
                        writer.WriteLine(input);
                    }
                }
                _view.ShowMessage($"Your text has been appended to {_model.FilePath}!");
            }
            SplitFile();
            PostSplitMenu();
        }
        private void SplitFile()
        {
                //if (!Directory.Exists("SplitFiles")) Directory.CreateDirectory("SplitFiles");

                string input = _view.GetUserInput("Enter the number of characters per small file: ");
                if (!int.TryParse(input, out int chunkSize) || chunkSize <= 0)
                {
                    _view.ShowMessage("Invalid input. Please enter a positive integer.");
                    return;
                }
                _model.ChunkSize = chunkSize;

                string content = File.ReadAllText(_model.FilePath).Replace("\r\n", "\n");
                int totalChars = content.Length;
                int fileCount = 1;
                int index = 0;
                int fileCountMax = (totalChars + _model.ChunkSize - 1) / _model.ChunkSize;
                int padding = fileCountMax.ToString().Length;

                while (index < totalChars)
                {
                    string smallFileName = Path.Combine(_model.SplitFolder, fileCount.ToString().PadLeft(padding, '0') + ".txt");
                    int length = Math.Min(_model.ChunkSize, totalChars - index);
                    string chunk = content.Substring(index, length).Replace("\n", Environment.NewLine);

                    File.WriteAllText(smallFileName, chunk);
                    _model.Messages.Add($"{smallFileName} created with {chunk.Length} visible characters.");

                    index += length;
                    fileCount++;
                }

                _model.Messages.Add("Splitting completed!");
                _view.DisplayMessages(_model);
        }

        private void PostSplitMenu()
        {
            while (true)
            {
                _view.ShowMessage("\nPost-split options:");
                _view.ShowMessage("1. View a specific split file");
                _view.ShowMessage("2. Delete a fragment");
                _view.ShowMessage("3. Defragment into output.txt");
                _view.ShowMessage("4. Exit");
                string choice = _view.GetUserInput("Choose an option: ");

                switch (choice)
                {
                    case "1":
                        ViewFragment();
                        break;
                    case "2":
                        DeleteFragment();
                        break;
                    case "3":
                        DefragmentFiles();
                        break;
                    case "4":
                        _view.ShowMessage("Exiting...");
                        return;
                    default:
                        _view.ShowMessage("Invalid choice. Try again.");
                        break;
                }
            }
        }

        private void ViewFragment()
        {
            string filename = _view.GetUserInput("Enter the fragment file name to view: ");
            string fullPath = Path.Combine(_model.SplitFolder, filename);
            if (File.Exists(fullPath))
            {
                string content = File.ReadAllText(fullPath);
                _view.ShowMessage($"\n--- {filename} ---\n{content}\n--- End ---");
            }
            else
            {
                _view.ShowMessage("File does not exist.");
            }
        }

        private void DeleteFragment()
        {
            string filename = _view.GetUserInput("Enter the fragment file name to delete: ");
            string fullPath = Path.Combine(_model.SplitFolder, filename);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _view.ShowMessage($"{filename} deleted successfully.");
            }
            else
            {
                _view.ShowMessage("File does not exist.");
            }
        }

        private void DefragmentFiles()
        {
            var fragmentFiles = Directory.GetFiles(_model.SplitFolder)
                .Where(f => Path.GetFileName(f) != "output.txt")
                .OrderBy(f => f).ToArray();

            using (StreamWriter writer = new StreamWriter(_model.OutputFile, false))
            {
                foreach (var file in fragmentFiles)
                {
                    string content = File.ReadAllText(file);
                    writer.Write(content);
                }
            }

            _view.ShowMessage($"All fragments defragmented into {_model.OutputFile}");
        }
    }
}
