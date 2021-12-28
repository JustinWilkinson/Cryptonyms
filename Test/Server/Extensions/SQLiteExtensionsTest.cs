using Cryptonyms.Server.Extensions;
using Xunit;
using System.Data;
using System.Data.SQLite;

namespace Cryptonyms.Test.Server.Extensions
{
    public class SQLiteExtensionsTest
    {
        [Theory]
        [InlineData("@Value", "Value")]
        [InlineData("@IntValue", 1, DbType.Int32)]
        [InlineData("@IntValueAsString", 1)]
        public void AddParameter_AnySQLiteCommand_AddsParameterSuccessfully(string name, object value, DbType dbType = DbType.String)
        {
            // Arrange
            var command = new SQLiteCommand($"SELECT * FROM Table WHERE Column = {name};");

            // Act
            command.AddParameter(name, value, dbType);

            // Assert
            Assert.Equal(1, command.Parameters.Count);
            Assert.True(command.Parameters.Contains(name));
            Assert.Equal(value, command.Parameters[name].Value);
            Assert.Equal(dbType, command.Parameters[name].DbType);
        }

        [Fact]
        public void AddParameter_AnySQLiteCommand_AddsMultipleParameterSuccessfully()
        {
            // Arrange
            var command = new SQLiteCommand($"SELECT * FROM Table WHERE Column1 = @Value1 AND Column2 = @Value2;");

            // Act
            command.AddParameter("@Value1", "Value 1");
            command.AddParameter("@Value2", 1, DbType.Int32);

            // Assert
            Assert.Equal(2, command.Parameters.Count);
            Assert.True(command.Parameters.Contains("@Value1"));
            Assert.Equal("Value 1", command.Parameters["@Value1"].Value);
            Assert.Equal(DbType.String, command.Parameters["@Value1"].DbType);
            Assert.True(command.Parameters.Contains("@Value2"));
            Assert.Equal(1, command.Parameters["@Value2"].Value);
            Assert.Equal(DbType.Int32, command.Parameters["@Value2"].DbType);
        }
    }
}