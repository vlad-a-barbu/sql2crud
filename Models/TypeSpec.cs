namespace sql2crud.net;


/// <summary>
/// NVARCHAR(MAX) -> new TypeSpec(DataType.NVARCHAR, new List<string> { "MAX" })
/// DECIMAL(16, 4) -> new TypeSpec(DataType.DECIMAL, new List<string> { "16", "4" })
/// </summary>
/// <param name="DataType"></param>
/// <param name="Args"></param>
record TypeSpec(DataType DataType, List<string>? Args = null)
{
    public override string ToString()
    {
        return Args is not null
            ? $"{DataType.Name()}({string.Join(",", Args)})"
            : DataType.Name();
    }
}

