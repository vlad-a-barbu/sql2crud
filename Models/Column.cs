namespace sql2crud.net;

record Column(string Name, TypeSpec TypeSpec, List<ConstraintType> Constraints)
{
    public override string ToString()
    {
        return $"[{Name}] {TypeSpec} {string.Join(" ", Constraints.Select(x => x.Name()))}";
    }
}

