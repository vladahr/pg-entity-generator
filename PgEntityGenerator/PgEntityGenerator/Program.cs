// See https://aka.ms/new-console-template for more information
using PgEntityGenerator;
using System.Diagnostics;

if (args == default || args.Length < 3)
{
    Console.WriteLine($"Invalid call arguments, specify schema table connectionString");
    Console.WriteLine($"Example run is powershell:");
    Console.WriteLine(".\\PgEntityGenerator.exe \"tschema\" \"table\" \"Server=localhost;Port=5432;Database=test;User ID=postgres;Password=****;CommandTimeout=300;\"");

    Console.ReadKey();
    Environment.Exit(-1);
}

var schema = args[0];
var table = args[1];
var connectionString = string.Join(' ', args[2..]);

await using var tableInfoRetreiver = new PostgreTableInfoRetriever(connectionString);
var info = await tableInfoRetreiver.GetTableInfoAsync(schema, table);
Console.WriteLine($"Found {info.Columns.Count} columns in {info.SchemaName}.{info.TableName}");

var classFeature = NetClassBuilder.GetEntityClass(info);
var classMapFeature = NetClassBuilder.GetMapClass(info);

Console.WriteLine($"Creating files {classFeature.FileName}, {classMapFeature.FileName}");

var classPath = Path.GetFullPath($"./{classFeature.FileName}");
var classMapPath = Path.GetFullPath($"./{classMapFeature.FileName}");

await Task.WhenAll(
    File.WriteAllTextAsync(classPath, classFeature.Content),
    File.WriteAllTextAsync(classMapPath, classMapFeature.Content)
);


Process.Start(classPath);
Process.Start(classMapPath);

Console.WriteLine($"Task completed, exit in 5s.");
Thread.Sleep(5000);
