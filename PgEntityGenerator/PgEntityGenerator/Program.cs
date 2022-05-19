// See https://aka.ms/new-console-template for more information
using PgEntityGenerator;
using System.Diagnostics;
using System.Runtime.InteropServices;

if (args == default || args.Length < 3)
{
    Console.WriteLine($"Invalid call arguments, specify schema table connectionString");
    Console.WriteLine($"Example run is powershell:");
    Console.WriteLine(".\\PgEntityGenerator.exe \"Server=localhost;Port=5432;Database=test;User ID=postgres;Password=****;CommandTimeout=300;\" \"tschema\" \"table\"");

    Console.ReadKey();
    Environment.Exit(-1);
}

var connectionString = args[0];
var schema = args[1].ToLowerInvariant();
var table = args[2].ToLowerInvariant();

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

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    Process.Start("notepad.exe", classPath);
    Process.Start("notepad.exe", classMapPath);
}

Console.WriteLine($"Task completed, exit in 5s.");
Thread.Sleep(5000);
