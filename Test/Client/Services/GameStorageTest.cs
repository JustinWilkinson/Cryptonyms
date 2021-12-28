using Cloudcrate.AspNetCore.Blazor.Browser.Storage;
using Cryptonyms.Client.Services;
using Microsoft.JSInterop;
using Moq;
using Xunit;

namespace Cryptonyms.Test.Client.Services
{
    public class GameStorageTest
    {
        private const string DeviceId = "DeviceId";
        private const string PlayerName = "PlayerName";

        private readonly Mock<IJsRuntimeWrapper> _mockJSRuntime = new();
        private GameStorage _gameStorage;

        public GameStorageTest()
        {
            _mockJSRuntime.Reset();
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), DeviceId)).Returns(DeviceId);
            _mockJSRuntime.Setup(s => s.Invoke<string>(It.IsAny<string>(), PlayerName)).Returns(PlayerName);
            _gameStorage = new GameStorage(new LocalStorage(_mockJSRuntime.Object));
        }

        [Fact]
        public void GameStorage_NewDeviceId_CachesNewDeviceId()
        {
            // Arrange
            const string newDeviceId = "NewDeviceId";

            // Act
            var callOne = _gameStorage.DeviceId;
            _gameStorage.DeviceId = newDeviceId;
            var callTwo = _gameStorage.DeviceId;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<string>(It.IsAny<string>(), DeviceId), Times.Once);
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), DeviceId, newDeviceId), Times.Once);
            Assert.Equal(DeviceId, callOne);
            Assert.Equal(newDeviceId, callTwo);
        }


        [Fact]
        public void GameStorage_NewPlayerName_CachesNewPlayerName()
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
            Assert.Equal(PlayerName, callOne);
            Assert.Equal(newPlayerName, callTwo);
        }

        [Fact]
        public void GameStorage_SettingDeviceIdToNull_RemovesDeviceId()
        {
            // Act
            _gameStorage.DeviceId = null;

            // Assert
            _mockJSRuntime.Verify(s => s.Invoke<object>(It.IsAny<string>(), DeviceId), Times.Once);
        }

        [Fact]
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