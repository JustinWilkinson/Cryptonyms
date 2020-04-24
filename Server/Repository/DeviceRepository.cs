using Cryptonyms.Server.Extensions;
using Cryptonyms.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Cryptonyms.Server.Repository
{
    public interface IDeviceRepository
    {
        void AddOrUpdateDevice(string deviceId);

        IEnumerable<Device> GetDevices();

        void DeleteDevices(IEnumerable<string> deviceIds);
    }

    public class DeviceRepository : Repository, IDeviceRepository
    {
        private readonly ILogger<DeviceRepository> _logger;

        public DeviceRepository(ILogger<DeviceRepository> logger) : base("CREATE TABLE IF NOT EXISTS Devices (DeviceId text, LastSeenUtc text)")
        {
            _logger = logger;
        }

        public void AddOrUpdateDevice(string deviceId)
        {
            try
            {
                ExecuteInTransaction(connection =>
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
                _logger.LogError(ex, $"An error occurred updating the last seen time for device '{deviceId}'.");
                throw;
            }
        }

        public IEnumerable<Device> GetDevices()
        {
            try
            {
                return Execute("SELECT * FROM Devices", reader => new Device { DeviceId = reader["DeviceId"].ToString(), LastSeenUtc = Convert.ToDateTime(reader["LastSeenUtc"]) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred retrieving list of devices.");
                throw;
            }
        }

        public void DeleteDevices(IEnumerable<string> deviceIds)
        {
            try
            {
                ExecuteInTransaction(connection =>
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