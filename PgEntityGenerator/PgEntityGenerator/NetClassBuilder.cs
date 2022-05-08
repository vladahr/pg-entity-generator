using System.Text;
using PgEntityGenerator.Helpers;
using static PgEntityGenerator.PostgreTableInfoRetriever;

namespace PgEntityGenerator
{
    public class NetClassBuilder
    {
        public static ClassFeature GetEntityClass(TableInfo tableInfo)
        {
            var builder = new StringBuilder();
            var className = tableInfo.TableName.ToPascalCase();

            builder.AppendLine($"public class {className}Entity");
            builder.AppendLine("{");

            var iteration = 0;
            foreach (var column in tableInfo.Columns)
            {
                var propName = column.Name.ToPascalCase();
                var type = PostgreTypeMapper.GetNetType(column.PostgreType, column.IsNullable);

                if (iteration > 0)
                {
                    builder.AppendLine();
                }

                builder.AppendLine($"\tpublic virtual {type} {propName} {{ get; set; }}");
                iteration++;
            }

            builder.AppendLine("}");

            return new ClassFeature
            {
                FileName = $"{className}Entity.cs",
                Content = builder.ToString()
            };
        }

        public static ClassFeature GetMapClass(TableInfo tableInfo)
        {
            var builder = new StringBuilder();
            var className = tableInfo.TableName.ToPascalCase();

            builder.AppendLine($"public class {className}Map : ClassMap<{className}Entity>");
            builder.AppendLine("{");
            builder.AppendLine($"\tpublic {className}Map()");
            builder.AppendLine("\t{");

            builder.AppendLine($"\t\tSchema(\"{tableInfo.SchemaName}\");");
            builder.AppendLine($"\t\tTable(\"{tableInfo.TableName}\");");

            builder.AppendLine();
            foreach (var column in tableInfo.Columns)
            {
                var propName = column.Name.ToPascalCase();

                if (column.IsPrimaryKey)
                {
                    var idSuffix = string.Empty;
                    if (!string.IsNullOrWhiteSpace(column.DefaultValue) &&
                        column.DefaultValue.TryParseSequence(out var sequence)) 
                    {
                        idSuffix = $".GeneratedBy.Sequence(\"{sequence}\")";
                    }

                    builder.AppendLine($"\t\tId(x => x.{propName}){idSuffix};");
                }
                else
                {
                    builder.AppendLine($"\t\tMap(x => x.{propName}, \"{column.Name}\");");
                }
            }
            builder.AppendLine("\t}");
            builder.AppendLine("}");

            return new ClassFeature
            {
                FileName = $"{className}Map.cs",
                Content = builder.ToString()
            };
        }

        public class ClassFeature
        {
            public string FileName { get; init; }

            public string Content { get; init; }
        }
    }
}
