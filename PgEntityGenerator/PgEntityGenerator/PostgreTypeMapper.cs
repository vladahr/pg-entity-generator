namespace PgEntityGenerator
{
    public class PostgreTypeMapper
    {
        public static string GetNetType(string postgreType, bool isNullable)
        {
            if (postgreToNetType.TryGetValue(postgreType, out var result))
            {
                return isNullable ? result.Nullable : result.NonNullable;
            }

            Console.WriteLine($"Can't find .NET type for {postgreType}");
            return $"\"{postgreType}\"";
        }

        private static readonly Dictionary<string, NetVariant> postgreToNetType = new()
        {
            ["boolean"] = new("bool", "bool?"),
            ["smallint"] = new("short", "short?"),
            ["integer"] = new("int", "int?"),
            ["bigint"] = new("long", "long?"),
            ["real"] = new("float", "float?"),
            ["double precision"] = new("double", "double?"),
            ["numeric"] = new("decimal", "decimal?"),
            ["money"] = new("decimal", "decimal?"),
            ["text"] = new("string", "string"),
            ["character varying"] = new("string", "string"),
            ["character"] = new("string", "string"),
            ["citext"] = new("string", "string"),
            ["json"] = new("string", "string"),
            ["jsonb"] = new("string", "string"),
            ["xml"] = new("string", "string"),
            ["uuid"] = new("Guid", "Guid?"),
            ["bytea"] = new("byte[]", "byte[]"),
            ["timestamp without time zone"] = new("DateTime", "DateTime?"),
            ["timestamp with time zone"] = new("DateTime", "DateTime?"),
            ["date"] = new("DateTime", "DateTime?"),
            ["time without time zone"] = new("TimeSpan", "TimeSpan?"),
            ["time with time zone"] = new("DateTimeOffset", "DateTimeOffset?"),
            ["interval"] = new("TimeSpan", "TimeSpan?"),
            ["cidr"] = new("IPAddress", "IPAddress"),
            ["inet"] = new("IPAddress", "IPAddress"),
            ["macaddr"] = new("PhysicalAddress", "PhysicalAddress"),
            ["tsquery"] = new("NpgsqlTsQuery", "NpgsqlTsQuery?"),
            ["tsvector"] = new("NpgsqlTsVector", "NpgsqlTsVector?"),
            ["bit(1)"] = new("bool", "bool?"),
            ["bit(n)"] = new("BitArray", "BitArray"),
            ["bit varying"] = new("BitArray", "BitArray"),
            ["point"] = new("NpgsqlPoint", "NpgsqlPoint?"),
            ["lseg"] = new("NpgsqlLSeg", "NpgsqlLSeg?"),
            ["path"] = new("NpgsqlPath", "NpgsqlPath?"),
            ["polygon"] = new("NpgsqlPolygon", "NpgsqlPolygon?"),
            ["line"] = new("NpgsqlLine", "NpgsqlLine?"),
            ["circle"] = new("NpgsqlCircle", "NpgsqlCircle?"),
            ["box"] = new("NpgsqlBox", "NpgsqlBox?"),
            ["hstore"] = new("Dictionary<string, string>", "Dictionary<string, string>"),
            ["oid"] = new("uint", "uint?"),
            ["xid"] = new("uint", "uint?"),
            ["cid"] = new("uint", "uint?"),
            ["oidvector"] = new("uint[]", "uint[]"),
            ["name"] = new("string", "string"),
            ["(internal) char"] = new("char", "char?"),
            ["geometry (PostGIS)"] = new("PostgisGeometry", "PostgisGeometry?"),
            ["record"] = new("T[]", "T[]"),
            ["composite types"] = new("T", "T"),
            ["range types"] = new("NpgsqlRange<T>", "NpgsqlRange<T>"),
            ["multirange types (PG14)"] = new("NpgsqlRange<T>[]", "NpgsqlRange<T>[]"),
            ["enum types"] = new("TEnum", "TEnum?"),
            ["array types"] = new("List<T>", "List<T>"),
        };

        private class NetVariant
        {
            public string NonNullable { get; }

            public string Nullable { get; }

            public NetVariant(string nonNullable, string nullable)
            {
                this.NonNullable = nonNullable;
                this.Nullable = nullable;
            }
        }
    }
}
