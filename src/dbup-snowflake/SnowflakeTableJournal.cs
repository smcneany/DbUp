using System;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;

namespace DbUp.Snowflake
{
    /// <summary>
    /// An implementation of the <see cref="Engine.IJournal"/> interface which tracks version numbers for a 
    /// SQL Server database using a table called dbo.SchemaVersions.
    /// </summary>
    public class SnowflakeTableJournal : TableJournal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeTableJournal"/> class.
        /// </summary>
        /// <param name="connectionManager">The connection manager.</param>
        /// <param name="logger">The log.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="table">The table name.</param>
        /// <example>
        /// var journal = new TableJournal("Server=server;Database=database;Trusted_Connection=True", "dbo", "MyVersionTable");
        /// </example>
        public SnowflakeTableJournal(Func<IConnectionManager> connectionManager, Func<IUpgradeLog> logger, string schema, string table)
            : base(connectionManager, logger, new SnowflakeServerObjectParser(), schema, table)
        {
        }

        protected override string GetInsertJournalEntrySql(string @scriptName, string @applied)
        {
            return $"insert into {FqSchemaTableName} (ScriptName, Applied) values ({@scriptName}, {@applied})";
        }

        protected override string GetJournalEntriesSql()
        {
            return $"select ScriptName from {FqSchemaTableName} order by ScriptName";
        }

        protected override string CreateSchemaTableSql(string quotedPrimaryKeyName)
        {
            return

                $@"create or replace table {FqSchemaTableName} (
                    Id int not null primary key AUTOINCREMENT,
                    ScriptName string(255) not null,
                    Applied  TIMESTAMP_LTZ(9) not null DEFAULT(CURRENT_TIMESTAMP));";
        }
    }
}
