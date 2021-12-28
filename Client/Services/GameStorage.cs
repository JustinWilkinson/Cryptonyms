using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

namespace Cryptonyms.Client.Services
{
    public class GameStorage
    {
        private readonly LocalStorage _storage;

        public GameStorage(LocalStorage localStorage)
        {
            _storage = localStorage;
        }

        private string _deviceId = null;
        public string DeviceId
        {
            get => _deviceId ?? _storage.GetItem("DeviceId");
            set
            {
                _deviceId = value;
                if (value is not null)
                {
                    _storage.SetItem("DeviceId", value);
                }
                else
                {
                    _storage.RemoveItem("DeviceId");
                }
            }
        }

        private string _playerName = null;
        public string PlayerName
        {
            get => _playerName ?? _storage.GetItem("PlayerName");
            set
            {
                _playerName = value;
                if (value is not null)
                {
                    _storage.SetItem("PlayerName", value);
                }
                else
                {
                    _storage.RemoveItem("PlayerName");
                }
            }
        }
    }
}