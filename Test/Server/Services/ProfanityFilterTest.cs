using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Services;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cryptonyms.Test.Server.Services
{
    public class ProfanityFilterTest
    {
        private readonly Mock<IFileReader> _mockFileReader = new();
        private readonly Mock<IOptions<ApplicationOptions>> _mockOptions = new();
        private ProfanityFilter _profanityFilter;

        public ProfanityFilterTest()
        {
            _mockFileReader.Reset();
            _mockOptions.Reset();
            _mockFileReader.Setup(x => x.ReadLinesAsync(It.IsAny<string>())).Returns(new[] { "badword", "badwordes", "yucks", "yucky", "awfuley" }.ToAsyncEnumerable());
            _mockOptions.SetupGet(x => x.Value).Returns(new ApplicationOptions { ProfanitiesPath = "" });
            _profanityFilter = new ProfanityFilter(_mockFileReader.Object, _mockOptions.Object);
        }

        [Theory]
        [InlineData("This is ok", false)]
        [InlineData("badword", true)]
        [InlineData("baDwOrD", true)]
        [InlineData("badwords", true)]
        [InlineData("badwordes", true)]
        [InlineData("badwordeses", false)]
        [InlineData("this has a badword", true)]
        [InlineData("yucks", true)]
        [InlineData("yuckses", true)]
        [InlineData("yucky", true)]
        [InlineData("yuckies", true)]
        [InlineData("awfulies", true)]
        public async Task ContainsProfanity_GivenProfanityList_ReturnsExpected(string input, bool expectedResult)
            => Assert.Equal(expectedResult, await _profanityFilter.ContainsProfanityAsync(input));
    }
}