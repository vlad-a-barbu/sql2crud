using sql2crud.net;
using System.Text.Json;

if (args.Length < 1)
{
    Console.WriteLine("sql2crud needs the full path to your config.json");
    return;
}
var configContent = File.ReadAllText(args[0]);
var config = JsonSerializer.Deserialize<Config>(configContent)!;

Console.WriteLine($"Starting sql2crud generation from \"{config.ConnectionString}\"");
Console.WriteLine("Reading table metadata from DB");
var tables = new Reader(config.ConnectionString).Read(config.SearchEntities);

var createTableDir = Directory.CreateDirectory(config.SqlScriptsDirPath);
Console.WriteLine($"Writing create table sql scripts at \"{createTableDir.FullName}\"");
foreach (var table in tables)
{
    File.WriteAllText(Path.Combine(createTableDir.FullName, $"create_{table.Name}.sql"), table.ToString());
}

var writer = new Writer(
    tables,
    config.EntitiesNamespace,
    config.DTOsNamespace,
    config.ServicesNamespace,
    config.ControllersNamespace
);

Console.WriteLine($"Writing entities at \"{config.EntitiesDirPath}\"");
writer.WriteEntities(config.EntitiesDirPath);

Console.WriteLine($"Writing DTOs at \"{config.DTOsDirPath}\"");
writer.WriteDTOs(config.DTOsDirPath);

Console.WriteLine($"Writing services at \"{config.ServicesDirPath}\"");
writer.WriteServices(config.ServicesDirPath);

Console.WriteLine($"Writing controllers at \"{config.ControllersDirPath}\"");
writer.WriteControllers(config.ControllersDirPath);

Console.WriteLine("Done");
