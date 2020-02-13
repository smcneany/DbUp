using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using DbUp.Engine;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Support;
using Snowflake.Data.Client;

namespace DbUp.Snowflake
{
    /// <summary>
    /// An implementation of <see cref="ScriptExecutor"/> that executes against a SQL Server database.
    /// </summary>
    public class SnowflakeScriptExecutor : ScriptExecutor
    {
        /// <summary>
        /// Initializes an instance of the <see cref="SnowflakeScriptExecutor"/> class.
        /// </summary>
        /// <param name="connectionManagerFactory"></param>
        /// <param name="log">The logging mechanism.</param>
        /// <param name="schema">The schema that contains the table.</param>
        /// <param name="variablesEnabled">Function that returns <c>true</c> if variables should be replaced, <c>false</c> otherwise.</param>
        /// <param name="scriptPreprocessors">Script Preprocessors in addition to variable substitution</param>
        /// <param name="journalFactory">Database journal</param>
        public SnowflakeScriptExecutor(Func<IConnectionManager> connectionManagerFactory, Func<IUpgradeLog> log, string schema, Func<bool> variablesEnabled,
            IEnumerable<IScriptPreprocessor> scriptPreprocessors, Func<IJournal> journalFactory)
            : base(connectionManagerFactory, new SnowflakeServerObjectParser(), log, schema, variablesEnabled, scriptPreprocessors, journalFactory)
        {
        }

        protected override string GetVerifySchemaSql(string schema)
        {
            return string.Format(@"CREATE schema IF NOT EXISTS ""{0}"";", Schema);
        }

        protected override void ExecuteCommandsWithinExceptionHandler(int index, SqlScript script, Action executeCommand)
        {
            try
            {
                executeCommand();
            }
            catch (SnowflakeDbException snowflakeException)
            {
                Log().WriteInformation("Snowflake exception has occured in script: '{0}'", script.Name);
                Log().WriteError("Script block number: {0}; Error code {1}; Message: {2}", index, snowflakeException.ErrorCode, snowflakeException.Message);
                Log().WriteError(snowflakeException.ToString());
                throw;
            }
        }
    }
}
