using Microsoft.Data.SqlClient;
using System.Data;

namespace sql2crud.net;

class Reader(string connectionString)
{
    readonly string connectionString = connectionString;

    public List<Table> Read(List<string> searchEntities)
    {
        var tables = new List<Table>();

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        DataTable schemaTables = connection.GetSchema("Tables");
        foreach (DataRow row in schemaTables.Rows)
        {
            var schema = row["TABLE_SCHEMA"].ToString()!;
            var tableName = row["TABLE_NAME"].ToString()!;
            if (searchEntities.Count > 0 && !searchEntities.Any(x => string.Compare(x, tableName, ignoreCase: true) == 0))
                continue;

            var columns = new List<Column>();

            DataTable schemaColumns = connection.GetSchema("Columns", [null, schema, tableName]);
            DataTable indexColumns = connection.GetSchema("IndexColumns", [null, schema, tableName, null, null]);

            foreach (DataRow columnRow in schemaColumns.Rows)
            {
                var columnName = columnRow["COLUMN_NAME"].ToString()!;
                var dataType = columnRow["DATA_TYPE"].ToString()!;
                var isNullable = columnRow["IS_NULLABLE"].ToString()!;
                var charMaxLength = columnRow["CHARACTER_MAXIMUM_LENGTH"].ToString();
                var numericPrecision = columnRow["NUMERIC_PRECISION"].ToString();
                var numericScale = columnRow["NUMERIC_SCALE"].ToString();

                DataType enumDataType = Mappings.DataTypes.SingleOrDefault(x => string.Compare(x.Value, dataType, ignoreCase: true) == 0).Key;

                List<string>? args = null;

                if (!string.IsNullOrEmpty(charMaxLength)) 
                    args = [charMaxLength != "-1" ? charMaxLength : "MAX"];

                if (enumDataType == DataType.DECIMAL &&
                    !string.IsNullOrEmpty(numericPrecision) && 
                    !string.IsNullOrEmpty(numericScale))
                    args = [numericPrecision, numericScale];

                TypeSpec typeSpec = new(enumDataType, args);

                var constraints = new List<ConstraintType>();
                if (isNullable.Equals("YES", StringComparison.OrdinalIgnoreCase))
                    constraints.Add(ConstraintType.NULL);
                else
                    constraints.Add(ConstraintType.NOT_NULL);

                var foundPK = false;
                var foundUQ = false;
                foreach (DataRow indexRow in indexColumns.Rows)
                {
                    if (indexRow["COLUMN_NAME"].ToString() == columnName && (indexRow["CONSTRAINT_NAME"].ToString()?.StartsWith("PK") ?? false))
                    {
                        constraints.Add(ConstraintType.PK);
                        constraints.Remove(ConstraintType.NOT_NULL);
                        foundPK = true;
                    }
                    if (indexRow["COLUMN_NAME"].ToString() == columnName && (indexRow["CONSTRAINT_NAME"].ToString()?.StartsWith("UQ") ?? false))
                    {
                        constraints.Add(ConstraintType.UNIQUE);
                        foundUQ = true;
                    }
                    if (foundPK && foundUQ) break;
                }

                columns.Add(new Column(columnName, typeSpec, constraints));
            }

            tables.Add(new Table(schema, tableName, columns));
        }

        return tables;
    }
}
