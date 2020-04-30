using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Cryptonyms.Client.Services;
using Microsoft.JSInterop;
using Moq;
using NUnit.Framework;

namespace Cryptonyms.Test.Client.Services
{
    [TestFixture]
    public class GameStorageTest
    {
        private readonly Mock<IJsRuntimeWrapper> _mockJSRuntime = new Mock<IJsRuntimeWrapper>();
        private GameStorage _gameStorage;
        private const string DeviceId = "DeviceId";
        private const string PlayerName = "PlayerName";

        [SetUp]
        public void SetUp()
        {
            _mockJSRuntime.Reset();
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), DeviceId)).Returns(DeviceId);
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), PlayerName)).Returns(PlayerName);
            _gameStorage = new GameStorage(new LocalStorage(_mockJSRuntime.Object));
        }

        [Test]
        public void GameStorage_Caches_DeviceId()
        {
            // Arrange
            var newDeviceId = "NewDeviceId";

            // Act
            var callOne = _gameStorage.DeviceId;
            _gameStorage.DeviceId = newDeviceId;
            var callTwo = _gameStorage.DeviceId;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<string>(It.IsAny<string>(), DeviceId), Times.Once);
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), DeviceId, newDeviceId), Times.Once);
            Assert.AreEqual(DeviceId, callOne);
            Assert.AreEqual(newDeviceId, callTwo);
        }


        [Test]
        public void GameStorage_Caches_PlayerName()
        {
            // Arrange
            var newPlayerName = "NewPlayerName";

            // Act
            var callOne = _gameStorage.PlayerName;
            _gameStorage.PlayerName = newPlayerName;
            var callTwo = _gameStorage.PlayerName;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<string>(It.IsAny<string>(), PlayerName), Times.Once);
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), PlayerName, newPlayerName), Times.Once);
            Assert.AreEqual(PlayerName, callOne);
            Assert.AreEqual(newPlayerName, callTwo);
        }

        [Test]
        public void GameStorage_SettingDeviceIdToNull_RemovesDeviceId()
        {
            // Act
            _gameStorage.DeviceId = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), DeviceId), Times.Once);
        }

        [Test]
        public void GameStorage_SettingPlayerNameToNull_RemovesPlayerName()
        {
            // Act
            _gameStorage.PlayerName = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), PlayerName), Times.Once);
        }

        public interface IJsRuntimeWrapper : IJSRuntime, IJSInProcessRuntime
        {

        }
    }
}