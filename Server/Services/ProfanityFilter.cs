using Cryptonyms.Server.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Services
{
    public interface IProfanityFilter
    {
        ValueTask<bool> ContainsProfanityAsync(string str);
    }

    public class ProfanityFilter : IProfanityFilter
    {
        private readonly HashSet<string> _profanities = new(StringComparer.OrdinalIgnoreCase);
        private readonly IFileReader _fileReader;
        private readonly IOptions<ApplicationOptions> _options;

        public ProfanityFilter(IFileReader fileReader, IOptions<ApplicationOptions> options)
        {
            _fileReader = fileReader;
            _options = options;
        }

        public async ValueTask<bool> ContainsProfanityAsync(string str)
        {
            await InitializeAsync();
            return str.Split(' ').Any(word => _profanities.Contains(word)) || _profanities.Contains(str.Replace(" ", "").Replace("-", "").Replace("'", ""));
        }

        private async ValueTask InitializeAsync()
        {
            if (_profanities.Count > 0)
            {
                return;
            }

            await foreach (var profanity in _fileReader.ReadLinesAsync(_options.Value.ProfanitiesPath))
            {
                _profanities.Add(profanity);
                if (profanity.EndsWith("es"))
                {
                    continue;
                }
                else if (profanity.EndsWith('s'))
                {
                    _profanities.Add($"{profanity}es");
                }
                else if (profanity.EndsWith("ey"))
                {
                    _profanities.Add($"{profanity[0..^2]}ies");
                }
                else if (profanity.EndsWith('y'))
                {
                    _profanities.Add($"{profanity[0..^1]}ies");
                }
                else
                {
                    _profanities.Add($"{profanity}s");
                }
            }
        }
    }
}