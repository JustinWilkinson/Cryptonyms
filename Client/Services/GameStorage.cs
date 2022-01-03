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
            get => _deviceId ??= _storage.GetItem(nameof(DeviceId));
            set
            {
                _deviceId = value;
                if (value is not null)
                {
                    _storage.SetItem(nameof(DeviceId), value);
                }
                else
                {
                    _storage.RemoveItem(nameof(DeviceId));
                }
            }
        }

        private string _playerName = null;
        public string PlayerName
        {
            get => _playerName ??= _storage.GetItem(nameof(PlayerName));
            set
            {
                _playerName = value;
                if (value is not null)
                {
                    _storage.SetItem(nameof(PlayerName), value);
                }
                else
                {
                    _storage.RemoveItem(nameof(PlayerName));
                }
            }
        }

        private bool? _makePrivateGames;
        public bool MakePrivateGames
        {
            get
            {
                if (!_makePrivateGames.HasValue)
                {
                    var storedValue = _storage.GetItem(nameof(MakePrivateGames));
                    _makePrivateGames = storedValue is not null && bool.TryParse(storedValue.ToString(), out var value) && value;
                }

                return _makePrivateGames.Value;
            }
            set
            {
                _makePrivateGames = value;
                _storage.SetItem(nameof(MakePrivateGames), value.ToString());
            }
        }

        private bool? _allowZeroLinksInCreatedGames = null;
        public bool AllowZeroLinksInCreatedGames
        {
            get
            {
                if (!_allowZeroLinksInCreatedGames.HasValue)
                {
                    var storedValue = _storage.GetItem(nameof(AllowZeroLinksInCreatedGames));
                    _allowZeroLinksInCreatedGames = storedValue is not null && bool.TryParse(storedValue.ToString(), out var value) && value;
                }

                return _allowZeroLinksInCreatedGames.Value;
            }
            set
            {
                _allowZeroLinksInCreatedGames = value;
                _storage.SetItem(nameof(AllowZeroLinksInCreatedGames), value.ToString());
            }
        }

        private bool? _allowMultipleBonusGuessesInCreatedGames = null;
        public bool AllowMultipleBonusGuessesInCreatedGames
        {
            get
            {
                if (!_allowMultipleBonusGuessesInCreatedGames.HasValue)
                {
                    var storedValue = _storage.GetItem(nameof(AllowMultipleBonusGuessesInCreatedGames));
                    _allowMultipleBonusGuessesInCreatedGames = storedValue is not null && bool.TryParse(storedValue.ToString(), out var value) && value;
                }

                return _allowMultipleBonusGuessesInCreatedGames.Value;
            }
            set
            {
                _allowMultipleBonusGuessesInCreatedGames = value;
                _storage.SetItem(nameof(AllowMultipleBonusGuessesInCreatedGames), value.ToString());
            }
        }
    }
}