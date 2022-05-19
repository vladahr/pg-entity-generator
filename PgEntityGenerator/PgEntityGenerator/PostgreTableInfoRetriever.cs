using Npgsql;
using static PgEntityGenerator.PostgreTableInfoRetriever.TableInfo;

namespace PgEntityGenerator
{
    public class PostgreTableInfoRetriever : IAsyncDisposable
    {
        private readonly NpgsqlConnection connection;

        public PostgreTableInfoRetriever(string connectionString)
        {
            this.connection = new NpgsqlConnection(connectionString);
        }

        public ValueTask DisposeAsync() => connection.DisposeAsync();

        public async Task<TableInfo> GetTableInfoAsync(string schemaName, string tableName)
        {
            var query = @"SELECT   
                    c.column_name,
                    c.data_type,
                    c.is_nullable = 'YES' as is_nullable,
					c.column_default,
					(SELECT EXISTS (
						SELECT 1
						FROM information_schema.table_constraints tco
						JOIN information_schema.key_column_usage kcu 
     					ON kcu.constraint_name = tco.constraint_name
     					AND kcu.constraint_schema = tco.constraint_schema
						WHERE tco.constraint_type = 'PRIMARY KEY' 
						AND kcu.table_name = t.table_name 
						AND kcu.table_schema = t.table_schema
						AND kcu.column_name = c.column_name
					)) as is_primary_key
              FROM information_schema.tables t
              INNER JOIN information_schema.columns c 
			  	ON c.table_name = t.table_name AND c.table_schema = t.table_schema
              WHERE
                t.table_schema ilike @t_schema AND
                t.table_name ilike  @t_name";

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var cmd = new NpgsqlCommand(query, connection);
            cmd.Parameters.AddWithValue("t_schema", schemaName);
            cmd.Parameters.AddWithValue("t_name", tableName);

            var columnsInfo = await GetColumnsInfoAsync(cmd);

            return new TableInfo
            {
                SchemaName = schemaName,
                TableName = tableName,
                Columns = columnsInfo
            };
        }

        private async Task<List<ColumnsInfo>> GetColumnsInfoAsync(NpgsqlCommand cmd)
        {
            await using var reader = await cmd.ExecuteReaderAsync();

            var columnsList = new List<ColumnsInfo>();
            while (await reader.ReadAsync())
            {
                var columns = new ColumnsInfo
                {
                    Name = reader.GetString(0),
                    PostgreType = reader.GetString(1),
                    IsNullable = reader.GetBoolean(2),
                    DefaultValue = reader.IsDBNull(3) ? null : reader.GetString(3),
                    IsPrimaryKey = reader.GetBoolean(4)
                };

                columnsList.Add(columns);
            }

            return columnsList;
        }

        public class TableInfo
        {
            public string SchemaName { get; init; }

            public string TableName { get; init; }

            public List<ColumnsInfo> Columns { get; init; }

            public class ColumnsInfo
            {
                public string Name { get; init; }

                public string PostgreType { get; init; }

                public bool IsNullable { get; init; }

                public string DefaultValue { get; init; }

                public bool IsPrimaryKey { get; init; }
            }
        }
    }
}
