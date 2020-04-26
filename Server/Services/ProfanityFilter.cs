using Cryptonyms.Server.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cryptonyms.Server.Services
{
    public interface IProfanityFilter
    {
        bool ContainsProfanity(string str);
    }

    public class ProfanityFilter : IProfanityFilter
    {
        private readonly HashSet<string> _profanities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ProfanityFilter(IFileReader fileReader, IOptions<ApplicationOptions> options)
        {
            foreach (var profanity in fileReader.ReadFileLines(options.Value.ProfanitiesPath))
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

        public bool ContainsProfanity(string str) => str.ToLower().Split(' ').Any(word => _profanities.Contains(word)) || _profanities.Contains(str.Replace(" ", "").Replace("-", "").Replace("'", ""));
    }
}