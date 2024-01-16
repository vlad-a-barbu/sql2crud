namespace sql2crud.net;

enum DataType
{
    NVARCHAR,
    INT,
    BIT,
    DATETIME,
    UNIQUEIDENTIFIER,
    DECIMAL
}

enum ConstraintType
{
    PK,
    NULL,
    NOT_NULL,
    UNIQUE
}

enum Placeholder
{
    Namespace,

}
