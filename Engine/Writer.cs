using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace sql2crud.net;

class Writer(
    List<Table> tables, 
    string entitiesNamespace,
    string dtosNamespace,
    string servicesNamespace,
    string controllersNamespace)
{
    private readonly List<Table> tables = tables;
    private readonly string entitiesNamespace = entitiesNamespace;
    private readonly string dtosNamespace = dtosNamespace;
    private readonly string servicesNamespace = servicesNamespace;
    private readonly string controllersNamespace = controllersNamespace;

    public void WriteEntities(string outputDirPath)
    {
        var outDir = Directory.CreateDirectory(outputDirPath);

        foreach (var table in tables)
        {
            var builder = new StringBuilder();

            builder.Append($@"
namespace {entitiesNamespace};

public record {table.Name}Model
{{
    {string.Join($"\n\t", table.Columns.Select(x => x.ToCSharpProperty()))}
}}
");

            File.WriteAllText(Path.Combine(outDir.FullName, $"{table.Name}Model.cs"), builder.ToString());
        }
    }
    
    public void WriteDTOs(string outputDirPath)
    {
        var outDir = Directory.CreateDirectory(outputDirPath);

        foreach (var table in tables)
        {
            var builder = new StringBuilder();
            builder.Append($@"
namespace {dtosNamespace};

public record Create{table.Name}Dto
{{
    {string.Join(
        $"\n\t",
        table.Columns.Where(x => x.Constraints.All(y => y != ConstraintType.PK)).Select(x => x.ToCSharpProperty()))}
}}
");
            File.WriteAllText(Path.Combine(outDir.FullName, $"Create{table.Name}Dto.cs"), builder.ToString());
            
            builder.Clear();
            builder.Append($@"
namespace {dtosNamespace};

public record Update{table.Name}Dto
{{
    {string.Join(
        $"\n\t", 
        table.Columns.Select(x => x.ToCSharpProperty()))}
}}
");
            File.WriteAllText(Path.Combine(outDir.FullName, $"Update{table.Name}Dto.cs"), builder.ToString());
        }
    }

    public void WriteServices(string outputDirPath)
    {
        var outDir = Directory.CreateDirectory(outputDirPath);

        foreach (var table in tables)
        {
            var template = GetTemplate(ServiceTemplateResource);

            foreach (var replacement in Replacements)
            {
                template = Regex.Replace(template, replacement.Key, replacement.Value(table));
            }

            File.WriteAllText(Path.Combine(outDir.FullName, $"I{table.Name}Service.cs"), template);
        }
    }

    public void WriteControllers(string outputDirPath)
    {
        var outDir = Directory.CreateDirectory(outputDirPath);

        foreach (var table in tables)
        {
            var template = GetTemplate(ControllerTemplateResource);

            foreach (var replacement in Replacements)
            {
                template = Regex.Replace(template, replacement.Key, replacement.Value(table));
            }

            File.WriteAllText(Path.Combine(outDir.FullName, $"{table.Name}Controller.cs"), template);
        }
    }

    private const string ControllerTemplateResource = "sql2crud.net.Templates.ControllerTemplate_async.txt";
    private const string ServiceTemplateResource = "sql2crud.net.Templates.ServiceTemplate.txt";

    private static string GetTemplate(string resourceName)
    {
        var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    record GeneratedFile(string FileName, string Content);

    private readonly Dictionary<string, Func<Table, string>> Replacements = new()
    {
        ["{{ENTITIES_NAMESPACE}}"] = _ => entitiesNamespace,
        ["{{DTO_NAMESPACE}}"] = _ => dtosNamespace,
        ["{{SERVICES_NAMESPACE}}"] = _ => servicesNamespace,
        ["{{CONTROLLERS_NAMESPACE}}"] = _ => controllersNamespace,
        ["{{PASCAL_TABLE_NAME}}"] = table => table.Name.ToPascalCase(),
        ["{{PASCAL_TABLE_NAME_PLURAL}}"] = table => table.Name.UnfuckPlural().ToPascalCase(),
        ["{{CAMEL_TABLE_NAME}}"] = table => table.Name.ToCamelCase(),
        ["{{PRIMARY_KEY_ROUTE}}"] = table => table.GetPrimaryKeyAsParams(includeTypes: true, "[FromRoute] "),
        ["{{PRIMARY_KEY_QUERY}}"] = table => table.GetPrimaryKeyAsParams(includeTypes: true, "[FromQuery] "),
        ["{{PRIMARY_KEY}}"] = table => table.GetPrimaryKeyAsParams(includeTypes: true),
        ["{{PRIMARY_KEY_NO_TYPES}}"] = table => table.GetPrimaryKeyAsParams(includeTypes: false),
    };
}
