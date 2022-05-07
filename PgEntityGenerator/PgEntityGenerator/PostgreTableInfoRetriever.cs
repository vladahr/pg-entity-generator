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
                            c.is_nullable
              FROM information_schema.tables t
              INNER JOIN information_schema.columns c ON c.table_name = t.table_name 
                                AND c.table_schema = t.table_schema
              WHERE
                --t.table_schema NOT IN ('information_schema', 'pg_catalog') AND
                --t.table_type = 'BASE TABLE' AND
                t.table_schema = @t_schema AND
                t.table_name = @t_name";

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
                    IsNullable = reader.GetString(2).Equals("YES", StringComparison.OrdinalIgnoreCase)
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
            }
        }
    }
}
