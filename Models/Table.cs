namespace sql2crud.net;

record Table(string Schema, string Name, List<Column> Columns)
{
    public override string ToString()
    {
        return @$"
CREATE TABLE [{Schema}].[{Name}] (
    {string.Join($",\n\t", Columns)}
);";
    }
}
