namespace sql2crud.net;

static class Mappings
{
    public static readonly Dictionary<DataType, string> DataTypes = new()
    {
        [DataType.NVARCHAR] = "NVARCHAR",
        [DataType.INT] = "INT",
        [DataType.BIT] = "BIT",
        [DataType.DATETIME] = "DATETIME",
        [DataType.UNIQUEIDENTIFIER] = "UNIQUEIDENTIFIER",
        [DataType.DECIMAL] = "DECIMAL",
    };

    public static readonly Dictionary<ConstraintType, string> ConstraintTypes = new()
    {
        [ConstraintType.PK] = "PRIMARY KEY",
        [ConstraintType.NULL] = "NULL",
        [ConstraintType.NOT_NULL] = "NOT NULL",
        [ConstraintType.UNIQUE] = "UNIQUE"
    };

    public static readonly Dictionary<DataType, string> CSharpDataTypes = new()
    {
        [DataType.NVARCHAR] = "string",
        [DataType.INT] = "int",
        [DataType.BIT] = "bool",
        [DataType.DATETIME] = "DateTime",
        [DataType.UNIQUEIDENTIFIER] = "Guid",
        [DataType.DECIMAL] = "decimal",
    };
}
