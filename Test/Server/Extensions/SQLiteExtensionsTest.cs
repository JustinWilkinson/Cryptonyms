using Codenames.Server.Extensions;
using NUnit.Framework;
using System.Data;
using System.Data.SQLite;

namespace Codenames.Test.Server.Extensions
{
    public class SQLiteExtensionsTest
    {
        [TestCase("@Value", "Value")]
        [TestCase("@IntValue", 1, DbType.Int32)]
        [TestCase("@IntValueAsString", 1)]
        public void AddParameter_AnySQLiteCommand_AddsParameterSuccessfully(string name, object value, DbType dbType = DbType.String)
        {
            // Arrange
            var command = new SQLiteCommand($"SELECT * FROM Table WHERE Column = {name};");

            // Act
            command.AddParameter(name, value, dbType);

            // Assert
            Assert.AreEqual(1, command.Parameters.Count);
            Assert.True(command.Parameters.Contains(name));
            Assert.AreEqual(value, command.Parameters[name].Value);
            Assert.AreEqual(dbType, command.Parameters[name].DbType);
        }

        [Test]
        public void AddParameter_AnySQLiteCommand_AddsMultipleParameterSuccessfully()
        {
            // Arrange
            var command = new SQLiteCommand($"SELECT * FROM Table WHERE Column1 = @Value1 AND Column2 = @Value2;");

            // Act
            command.AddParameter("@Value1", "Value 1");
            command.AddParameter("@Value2", 1, DbType.Int32);

            // Assert
            Assert.AreEqual(2, command.Parameters.Count);
            Assert.True(command.Parameters.Contains("@Value1"));
            Assert.AreEqual("Value 1", command.Parameters["@Value1"].Value);
            Assert.AreEqual(DbType.String, command.Parameters["@Value1"].DbType);
            Assert.True(command.Parameters.Contains("@Value2"));
            Assert.AreEqual(1, command.Parameters["@Value2"].Value);
            Assert.AreEqual(DbType.Int32, command.Parameters["@Value2"].DbType);
        }
    }
}