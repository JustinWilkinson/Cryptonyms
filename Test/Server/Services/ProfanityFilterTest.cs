using Cryptonyms.Server.Configuration;
using Cryptonyms.Server.Extensions;
using Cryptonyms.Server.Services;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Cryptonyms.Test.Server.Services
{
    [TestFixture]
    public class ProfanityFilterTest
    {
        private readonly Mock<IFileReader> _mockFileReader = new();
        private readonly Mock<IOptions<ApplicationOptions>> _mockOptions = new();
        private ProfanityFilter _profanityFilter;

        [SetUp]
        public void SetUp()
        {
            _mockFileReader.Reset();
            _mockOptions.Reset();
            _mockFileReader.Setup(x => x.ReadLinesAsync(It.IsAny<string>())).Returns(new[] { "badword", "badwordes", "yucks", "yucky", "awfuley" }.ToAsyncEnumerable());
            _mockOptions.SetupGet(x => x.Value).Returns(new ApplicationOptions { ProfanitiesPath = "" });
            _profanityFilter = new ProfanityFilter(_mockFileReader.Object, _mockOptions.Object);
        }

        [TestCase("This is ok", ExpectedResult = false)]
        [TestCase("badword", ExpectedResult = true)]
        [TestCase("baDwOrD", ExpectedResult = true)]
        [TestCase("badwords", ExpectedResult = true)]
        [TestCase("badwordes", ExpectedResult = true)]
        [TestCase("badwordeses", ExpectedResult = false)]
        [TestCase("this has a badword", ExpectedResult = true)]
        [TestCase("yucks", ExpectedResult = true)]
        [TestCase("yuckses", ExpectedResult = true)]
        [TestCase("yucky", ExpectedResult = true)]
        [TestCase("yuckies", ExpectedResult = true)]
        [TestCase("awfulies", ExpectedResult = true)]
        public ValueTask<bool> ContainsProfanityAsync_GivenProfanityList_ReturnsExpected(string input) 
            => _profanityFilter.ContainsProfanityAsync(input);
    }
}