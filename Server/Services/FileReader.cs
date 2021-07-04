using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cryptonyms.Server.Services
{
    public interface IFileReader
    {
        IAsyncEnumerable<string> ReadLinesAsync(string path);
    }


    public class FileReader : IFileReader
    {
        private static readonly string _basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public async IAsyncEnumerable<string> ReadLinesAsync(string path)
        {
            await using var file = File.OpenRead(GetFullPath(path));
            using var reader = new StreamReader(file);

            string line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                yield return line;
            }
        }

        private static string GetFullPath(string relativePath) => Path.Combine(_basePath, relativePath);
    }
}