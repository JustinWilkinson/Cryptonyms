using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cryptonyms.Server.Services
{
    public interface IFileReader
    {
        string GetFullPath(string relativePath);

        IEnumerable<string> ReadFileLines(string path);

        IEnumerable<string> ReadFile(string path, string separator);
    }


    public class FileReader : IFileReader
    {
        private static readonly string _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public string GetFullPath(string relativePath) => Path.Combine(_basePath, relativePath);

        public IEnumerable<string> ReadFileLines(string path) => File.ReadLines(GetFullPath(path));

        public IEnumerable<string> ReadFile(string path, string separator) => File.ReadAllText(GetFullPath(path)).Split(separator, StringSplitOptions.RemoveEmptyEntries);
    }
}