using System.Data;
using System.Data.SQLite;

namespace Cryptonyms.Server.Extensions
{
    /// <summary>
    /// Contains useful extensions to System.Data.SQLite
    /// </summary>
    public static class SQLiteExtensions
    {
        /// <summary>
        /// Adds a parameter to the specified command.
        /// </summary>
        /// <param name="command">Command to add parameter to</param>
        /// <param name="parameterName">The name to give to the parameter</param>
        /// <param name="parameterValue">The value to assign to the parameter</param>
        /// <param name="dbType">The DbType of the parameter, defaults to DbType.String</param>
        public static void AddParameter(this SQLiteCommand command, string parameterName, object parameterValue, DbType dbType = DbType.String)
        {
            command.Parameters.Add(new SQLiteParameter(parameterName, parameterValue) { DbType = dbType });
        }
    }
}