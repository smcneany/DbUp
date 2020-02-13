using System;
using System.Data;
using DbUp;
using DbUp.Builder;
using DbUp.Engine.Output;
using DbUp.Engine.Transactions;
using DbUp.Snowflake;
using Snowflake.Data.Client;

/// <summary>
/// Configuration extension methods for Snowflake.
/// </summary>
// NOTE: DO NOT MOVE THIS TO A NAMESPACE
// Since the class just contains extension methods, we leave it in the global:: namespace so that it is always available
// ReSharper disable CheckNamespace
public static class SnowflakeServerExtensions
// ReSharper restore CheckNamespace
{
    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SnowflakeDatabase(this SupportedDatabases supported, string connectionString)
    {
        return SnowflakeDatabase(supported, connectionString, null);
    }

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SnowflakeDatabase(this SupportedDatabases supported, string connectionString, string schema)
    {
        return SnowflakeDatabase(new SnowflakeConnectionManager(connectionString), schema);
    }


    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SqlConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    public static UpgradeEngineBuilder SnowflakeDatabase(this SupportedDatabases supported, IConnectionManager connectionManager, string schema = null)
        => SnowflakeDatabase(connectionManager, schema);

    /// <summary>
    /// Creates an upgrader for SQL Server databases.
    /// </summary>
    /// <param name="connectionManager">The <see cref="IConnectionManager"/> to be used during a database
    /// upgrade. See <see cref="SnowflakeConnectionManager"/> for an example implementation</param>
    /// <param name="schema">The SQL schema name to use. Defaults to 'dbo'.</param>
    /// <returns>
    /// A builder for a database upgrader designed for SQL Server databases.
    /// </returns>
    static UpgradeEngineBuilder SnowflakeDatabase(IConnectionManager connectionManager, string schema)
    {
        var builder = new UpgradeEngineBuilder();
        builder.Configure(c => c.ConnectionManager = connectionManager);
        builder.Configure(c => c.ScriptExecutor = new SnowflakeScriptExecutor(() => c.ConnectionManager, () => c.Log, schema, () => c.VariablesEnabled, c.ScriptPreprocessors, () => c.Journal));
        builder.Configure(c => c.Journal = new SnowflakeTableJournal(() => c.ConnectionManager, () => c.Log, schema, "SchemaVersions"));
        return builder;
    }

    /// <summary>
    /// Tracks the list of executed scripts in a SQL Server table.
    /// </summary>
    /// <param name="builder">The builder.</param>
    /// <param name="schema">The schema.</param>
    /// <param name="table">The table.</param>
    /// <returns></returns>
    public static UpgradeEngineBuilder JournalToSqlTable(this UpgradeEngineBuilder builder, string schema, string table)
    {
        builder.Configure(c => c.Journal = new SnowflakeTableJournal(() => c.ConnectionManager, () => c.Log, schema, table));
        return builder;
    }


    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }


    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, string collation)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog(), collation: collation);
    }

    

    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForEnsureDatabase supported, string connectionString, int commandTimeout, string collation)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout, collation: collation);
    }




    /// <summary>
    /// Ensures that the database specified in the connection string exists.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for creating a database in case you're encountering a time out in this operation.</param>
    /// <param name="collation">The collation name to set during database creation</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(
        this SupportedDatabasesForEnsureDatabase supported,
        string connectionString,
        IUpgradeLog logger,
        int timeout = -1,
        string collation = null)
    {
        using (var connection = new SnowflakeDbConnection())
        {
            connection.ConnectionString = connectionString;
            try
            {
                connection.Open();
            }
            catch (SnowflakeDbException)
            {
                //// Failed to connect to master, lets try direct  
                //if (DatabaseExistsIfConnectedToDirectly(logger, connectionString, connection.Database))
                //    return;

                throw;
            }

            var sqlCommandText = $@"create database IF NOT EXISTS ""{connection.Database}"";";

            // Create the database...
            using (var command = new SnowflakeDbCommand()
            {
                CommandText = sqlCommandText,
                Connection = connection,
                CommandType = CommandType.Text
            })
            {
                if (timeout >= 0)
                {
                    command.CommandTimeout = timeout;
                }

                command.ExecuteNonQuery();
            }

            logger.WriteInformation(@"Created database {0}", connection.Database);
        }
    }

    //static bool DatabaseExistsIfConnectedToDirectly(IUpgradeLog logger, string connectionString, string databaseName)
    //{
    //    try
    //    {
    //        using (var connection = new SnowflakeDbConnection())
    //        {
    //            connection.ConnectionString = connectionString;
    //            connection.Open();
    //            return DatabaseExists(connection, databaseName);
    //        }
    //    }
    //    catch
    //    {
    //        logger.WriteInformation("Could not connect to the database directly");
    //        return false;
    //    }
    //}

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog());
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="commandTimeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, int commandTimeout)
    {
        SnowflakeDatabase(supported, connectionString, new ConsoleUpgradeLog(), commandTimeout);
    }

    /// <summary>
    /// Drop the database specified in the connection string.
    /// </summary>
    /// <param name="supported">Fluent helper type.</param>
    /// <param name="connectionString">The connection string.</param>
    /// <param name="logger">The <see cref="DbUp.Engine.Output.IUpgradeLog"/> used to record actions.</param>
    /// <param name="timeout">Use this to set the command time out for dropping a database in case you're encountering a time out in this operation.</param>
    /// <returns></returns>
    public static void SnowflakeDatabase(this SupportedDatabasesForDropDatabase supported, string connectionString, IUpgradeLog logger, int timeout = -1)
    {
        throw new NotImplementedException();

        //GetMasterConnectionStringBuilder(connectionString, logger, out var masterConnectionString, out var databaseName);

        //using (var connection = new SnowflakeDbConnection())
        //{
        //    connection.ConnectionString = masterConnectionString;
        //    connection.Open();
        //    if (!DatabaseExists(connection, databaseName))
        //        return;

        //    var dropDatabaseCommand = new SqlCommand($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}];", connection) { CommandType = CommandType.Text };
        //    using (var command = dropDatabaseCommand)
        //    {
        //        command.ExecuteNonQuery();
        //    }

        //    logger.WriteInformation("Dropped database {0}", databaseName);
        //}
    }

    //static bool DatabaseExists(SnowflakeDbConnection connection, string databaseName)
    //{
    //    var sqlCommandText = string.Format
    //    (
    //        @"SELECT TOP 1 case WHEN dbid IS NOT NULL THEN 1 ELSE 0 end FROM sys.sysdatabases WHERE name = '{0}';",
    //        databaseName
    //    );

    //    // check to see if the database already exists..
    //    using (var command = new SnowflakeDbCommand()
    //    {
    //        CommandText = sqlCommandText,
    //        Connection = connection,
    //        CommandType = CommandType.Text
    //    })

    //    {
    //        var results = (int?)command.ExecuteScalar();

    //        if (results.HasValue && results.Value == 1)
    //            return true;
    //        else
    //            return false;
    //    }
    //}
}
