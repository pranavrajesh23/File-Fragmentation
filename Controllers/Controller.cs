using System;
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
            if (!File.Exists(_model.FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_model.FilePath) ?? string.Empty);
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
            else
            {
                if (!Directory.Exists("SplitFiles")) Directory.CreateDirectory("SplitFiles");

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
                    string smallFileName = Path.Combine("SplitFiles", fileCount.ToString().PadLeft(padding, '0') + ".txt");
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
        }
    }
}
