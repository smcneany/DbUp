using System;
using System.Linq;
using DbUp.Support;

namespace DbUp.Snowflake
{
    /// <summary>
    /// Parses Snowflake Objects and performs quoting functions
    /// </summary>
    public class SnowflakeServerObjectParser : SqlObjectParser
    {

        public SnowflakeServerObjectParser()
            : base("\"", "\"")
        {
        }
    }
}
