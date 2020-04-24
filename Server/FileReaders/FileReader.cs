using System;
using System.Collections.Generic;
using System.IO;

namespace Cryptonyms.Server.FileReaders
{
    public interface IFileReader
    {
        IEnumerable<string> ReadFileLines(string path);

        IEnumerable<string> ReadFile(string path, string separator);
    }


    public class FileReader : IFileReader
    {
        public IEnumerable<string> ReadFileLines(string path) => File.ReadLines(path);

        public IEnumerable<string> ReadFile(string path, string separator) => File.ReadAllText(path).Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }
}