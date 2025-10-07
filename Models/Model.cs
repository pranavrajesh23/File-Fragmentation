using System.Collections.Generic;

namespace FileFragmentationConsole
{
    public class FileFragmentationModel
    {
        public string FilePath { get; set; } = "IOFiles/Input.txt";
        public int ChunkSize { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public string SplitFolder { get; set; } = "SplitFiles";
        public string OutputFile { get; set; } = "IOFiles/Output.txt";
    }
}
