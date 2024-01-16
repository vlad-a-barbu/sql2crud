using System.Text;

namespace sql2crud.net;

static class Extensions
{
    public static string Name(this ConstraintType constraintType) 
        => Mappings.ConstraintTypes[constraintType];

    public static string Name(this DataType dataType) 
        => Mappings.DataTypes[dataType];

    public static bool IsNullable(this Column column)
        => column.Constraints.Any(x => x == ConstraintType.NULL);

    public static string TryMarkAsNullable(this Column column)
        => column.IsNullable() ? "?" : string.Empty;

    public static string ToCSharpProperty(this Column column)
        => $"public {Mappings.CSharpDataTypes[column.TypeSpec.DataType]}{column.TryMarkAsNullable()} {column.Name} {{ get; set; }}";

    public static string ToCamelCase(this string str)
        => $"{str[0].ToString().ToLower()}{str[1..]}";

    public static string ToPascalCase(this string str)
        => $"{str[0].ToString().ToUpper()}{str[1..]}";

    public static string GetPrimaryKeyAsParams(this Table table, bool includeTypes, string customPrefix = "")
    {
        var builder = new StringBuilder();
        foreach (var column in table.Columns.Where(x => x.Constraints.Any(y => y == ConstraintType.PK)))
        {
            var prefix = builder.Length > 0 ? ", " : string.Empty;
            builder.Append(includeTypes
                ? $"{prefix}{customPrefix}{Mappings.CSharpDataTypes[column.TypeSpec.DataType]} {column.Name.ToCamelCase()}"
                : $"{prefix}{customPrefix}{column.Name.ToCamelCase()}");
        }
        return builder.ToString();
    }

    public static string UnfuckPlural(this string name) => name.Last() == 'y' ? $"{name[..^1]}ies" : name;
}
