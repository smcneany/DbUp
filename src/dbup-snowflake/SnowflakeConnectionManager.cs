using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Snowflake.Data.Client;

namespace DbUp.Snowflake
{
    /// <summary>
    /// Manages Sql Database Connections
    /// </summary>
    public class SnowflakeConnectionManager : DatabaseConnectionManager
    {
        /// <summary>
        /// Manages Sql Database Connections
        /// </summary>
        /// <param name="connectionString"></param>
        public SnowflakeConnectionManager(string connectionString)
             : base(new DelegateConnectionFactory((log, dbManager) =>
             {
                 var conn = new SnowflakeDbConnection();
                 conn.ConnectionString = connectionString;

                 return conn;
             }))
        {
        }

        public override IEnumerable<string> SplitScriptIntoCommands(string scriptContents)
        {
            var scriptStatements =
                Regex.Split(scriptContents, "^\\s*;\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            return scriptStatements;
        }
    }
}
