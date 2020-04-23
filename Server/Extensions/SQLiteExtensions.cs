using System.Data;
using System.Data.SQLite;

namespace Cryptonyms.Server.Extensions
{
    public static class SQLiteExtensions
    {
        public static void AddParameter(this SQLiteCommand command, string parameterName, object parameterValue, DbType dbType = DbType.String)
        {
            command.Parameters.Add(new SQLiteParameter(parameterName, parameterValue) { DbType = dbType });
        }
    }
}