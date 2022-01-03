using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Cryptonyms.Server.Repository
{
    /// <summary>
    /// Interface for managing the Devices table.
    /// </summary>
    public interface IDeviceRepository
    {
        Task AddOrUpdateDeviceAsync(string deviceId);

        IAsyncEnumerable<Device> GetDevicesAsync();

        Task DeleteDevicesAsync(IEnumerable<string> deviceIds);
    }

    /// <summary>
    /// Manages the Devices table.
    /// </summary>
    public class DeviceRepository : Repository, IDeviceRepository
    {
        private readonly ILogger<DeviceRepository> _logger;

        protected override string CreateStatement { get; } = "CREATE TABLE IF NOT EXISTS Devices (DeviceId text, LastSeenUtc text)";

        public DeviceRepository(ILogger<DeviceRepository> logger)
        {
            _logger = logger;
        }

        public async Task AddOrUpdateDeviceAsync(string deviceId)
        {
            try
            {
                await ExecuteInTransactionAsync(connection =>
                {
                    var selectCommand = new SQLiteCommand("SELECT COUNT(*) AS Count FROM Devices WHERE DeviceId = @DeviceId", connection);
                    selectCommand.AddParameter("@DeviceId", deviceId);

                    var insertOrUpdateCommand = Convert.ToInt32(selectCommand.ExecuteScalar()) == 0 ?
                        new SQLiteCommand("INSERT INTO Devices (DeviceId, LastSeenUtc) VALUES (@DeviceId, @LastSeenUtc)", connection) :
                        new SQLiteCommand("UPDATE Devices SET LastSeenUtc = @LastSeenUtc WHERE DeviceId = @DeviceId", connection);
                    insertOrUpdateCommand.AddParameter("@DeviceId", deviceId);
                    insertOrUpdateCommand.AddParameter("@LastSeenUtc", DateTime.UtcNow);
                    insertOrUpdateCommand.ExecuteNonQuery();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred updating the last seen time for device '{deviceId}'.", deviceId);
                throw;
            }
        }

        public IAsyncEnumerable<Device> GetDevicesAsync()
        {
            try
            {
                return ExecuteAsync("SELECT * FROM Devices", reader => new Device { DeviceId = reader["DeviceId"].ToString(), LastSeenUtc = Convert.ToDateTime(reader["LastSeenUtc"]) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving list of devices.");
                throw;
            }
        }

        public async Task DeleteDevicesAsync(IEnumerable<string> deviceIds)
        {
            try
            {
                await ExecuteInTransactionAsync(connection =>
                {
                    foreach (var deviceId in deviceIds)
                    {
                        var command = new SQLiteCommand("DELETE FROM Devices WHERE DeviceId = @DeviceId", connection);
                        command.AddParameter("@DeviceId", deviceId);
                        command.ExecuteNonQuery();
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred deleting old devices.");
                throw;
            }
        }
    }
}